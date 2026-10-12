import { randomUUID } from "node:crypto";
import { writeFileSync } from "node:fs";
import { createServer, Server, Socket } from "node:net";
import { join } from "node:path";

const lineBreak = "\r\n";
const endOfData = `${lineBreak}.${lineBreak}`;
const headerBodySeparator = `${lineBreak}${lineBreak}`;
const recipientPattern = /^RCPT TO:\s*<([^>]+)>/i;
const quotedPrintableSoftBreakPattern = /=\r\n/g;
const quotedPrintableBytePattern = /=([0-9A-F]{2})/gi;
const transferEncodingPattern = /^content-transfer-encoding:\s*(\S+)/im;
const multipartBoundaryPattern =
  /^content-type:\s*multipart\/[^;]+;[\s\S]*?boundary="?([^";\r\n]+)"?/im;
const boundaryDelimiter = "--";
const leadingLineBreakPattern = /^\r\n/;

export interface ReceivedEmail {
  to: string[];
  body: string;
}

type EmailReceiver = (email: ReceivedEmail) => void;

function decodeQuotedPrintable(encodedBody: string): string {
  const bytes = encodedBody
    .replace(quotedPrintableSoftBreakPattern, "")
    .replace(quotedPrintableBytePattern, (_, hexadecimal: string) =>
      String.fromCharCode(Number.parseInt(hexadecimal, 16)),
    );
  return Buffer.from(bytes, "latin1").toString("utf8");
}

function decodeParts(encodedBody: string, boundary: string): string {
  return encodedBody
    .split(`${boundaryDelimiter}${boundary}`)
    .slice(1, -1)
    .map((part) => decodeBody(part.replace(leadingLineBreakPattern, "")))
    .join(lineBreak);
}

function decodeBody(message: string): string {
  const separatorIndex = message.indexOf(headerBodySeparator);
  const headers = message.slice(0, separatorIndex);
  const encodedBody = message.slice(separatorIndex + headerBodySeparator.length);
  const boundary = multipartBoundaryPattern.exec(headers)?.[1];
  if (boundary !== undefined) {
    return decodeParts(encodedBody, boundary);
  }
  const transferEncoding = transferEncodingPattern.exec(headers)?.[1]?.toLowerCase();
  if (transferEncoding === "quoted-printable") {
    return decodeQuotedPrintable(encodedBody);
  }
  if (transferEncoding === "base64") {
    return Buffer.from(encodedBody, "base64").toString("utf8");
  }
  return encodedBody;
}

function serveConnection(socket: Socket, receive: EmailReceiver): void {
  let buffer = "";
  let isReadingData = false;
  let recipients: string[] = [];
  const reply = (line: string) => socket.write(`${line}${lineBreak}`);

  reply("220 e2e smtp sink");
  socket.on("data", (chunk) => {
    buffer += chunk.toString("latin1");
    for (;;) {
      if (isReadingData) {
        const dataEnd = buffer.indexOf(endOfData);
        if (dataEnd === -1) {
          return;
        }
        receive({ to: recipients, body: decodeBody(buffer.slice(0, dataEnd)) });
        buffer = buffer.slice(dataEnd + endOfData.length);
        isReadingData = false;
        recipients = [];
        reply("250 queued");
        continue;
      }
      const lineEnd = buffer.indexOf(lineBreak);
      if (lineEnd === -1) {
        return;
      }
      const command = buffer.slice(0, lineEnd);
      buffer = buffer.slice(lineEnd + lineBreak.length);
      const verb = command.slice(0, 4).toUpperCase();
      if (verb === "EHLO" || verb === "HELO") {
        reply("250 e2e smtp sink");
      } else if (verb === "RCPT") {
        const recipient = recipientPattern.exec(command)?.[1];
        if (recipient !== undefined) {
          recipients.push(recipient);
        }
        reply("250 ok");
      } else if (verb === "DATA") {
        isReadingData = true;
        reply("354 end with <CRLF>.<CRLF>");
      } else if (verb === "QUIT") {
        reply("221 bye");
        socket.end();
        return;
      } else {
        reply("250 ok");
      }
    }
  });
}

export class SmtpSink {
  private readonly server: Server;

  constructor(mailboxDirectory: string) {
    this.server = createServer((socket) =>
      serveConnection(socket, (email) =>
        writeFileSync(
          join(mailboxDirectory, `${Date.now()}-${randomUUID()}.json`),
          JSON.stringify(email),
        ),
      ),
    );
  }

  async start(port: number): Promise<void> {
    await new Promise<void>((resolve, reject) => {
      this.server.once("error", reject);
      this.server.listen(port, resolve);
    });
  }

  async stop(): Promise<void> {
    await new Promise<void>((resolve) => this.server.close(() => resolve()));
  }
}
