import { createServer, Server, Socket } from "node:net";

const lineBreak = "\r\n";
const endOfData = `${lineBreak}.${lineBreak}`;
const headerBodySeparator = `${lineBreak}${lineBreak}`;
const pollIntervalMilliseconds = 100;
const defaultWaitMilliseconds = 15_000;
const recipientPattern = /^RCPT TO:\s*<([^>]+)>/i;
const quotedPrintableSoftBreakPattern = /=\r\n/g;
const quotedPrintableBytePattern = /=([0-9A-F]{2})/gi;
const transferEncodingPattern = /^content-transfer-encoding:\s*(\S+)/im;

export interface ReceivedEmail {
  to: string[];
  body: string;
}

function decodeQuotedPrintable(encodedBody: string): string {
  const bytes = encodedBody
    .replace(quotedPrintableSoftBreakPattern, "")
    .replace(quotedPrintableBytePattern, (_, hexadecimal: string) =>
      String.fromCharCode(Number.parseInt(hexadecimal, 16)),
    );
  return Buffer.from(bytes, "latin1").toString("utf8");
}

function decodeBody(message: string): string {
  const separatorIndex = message.indexOf(headerBodySeparator);
  const headers = message.slice(0, separatorIndex);
  const encodedBody = message.slice(separatorIndex + headerBodySeparator.length);
  const transferEncoding = transferEncodingPattern.exec(headers)?.[1]?.toLowerCase();
  if (transferEncoding === "quoted-printable") {
    return decodeQuotedPrintable(encodedBody);
  }
  if (transferEncoding === "base64") {
    return Buffer.from(encodedBody, "base64").toString("utf8");
  }
  return encodedBody;
}

function serveConnection(socket: Socket, received: ReceivedEmail[]): void {
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
        received.push({ to: recipients, body: decodeBody(buffer.slice(0, dataEnd)) });
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
  private readonly received: ReceivedEmail[] = [];
  private readonly server: Server = createServer((socket) =>
    serveConnection(socket, this.received),
  );

  async start(port: number): Promise<void> {
    await new Promise<void>((resolve, reject) => {
      this.server.once("error", reject);
      this.server.listen(port, resolve);
    });
  }

  async stop(): Promise<void> {
    await new Promise<void>((resolve) => this.server.close(() => resolve()));
  }

  async waitForEmailTo(
    recipient: string,
    timeoutMilliseconds = defaultWaitMilliseconds,
  ): Promise<ReceivedEmail> {
    const deadline = Date.now() + timeoutMilliseconds;
    while (Date.now() < deadline) {
      const email = this.received.find((candidate) => candidate.to.includes(recipient));
      if (email !== undefined) {
        return email;
      }
      await new Promise((resolve) => setTimeout(resolve, pollIntervalMilliseconds));
    }
    throw new Error(`No email reached ${recipient} within ${timeoutMilliseconds} ms.`);
  }
}
