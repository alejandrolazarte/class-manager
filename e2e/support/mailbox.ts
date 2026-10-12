import { expect } from "@playwright/test";
import { readdirSync, readFileSync } from "node:fs";
import { join } from "node:path";
import { e2eEnvironment } from "./environment";
import { ReceivedEmail } from "./smtpSink";

const pollIntervalMilliseconds = 100;
const defaultWaitMilliseconds = 15_000;

function receivedEmails(): ReceivedEmail[] {
  return readdirSync(e2eEnvironment.mailboxDirectory)
    .sort()
    .map(
      (fileName) =>
        JSON.parse(
          readFileSync(join(e2eEnvironment.mailboxDirectory, fileName), "utf8"),
        ) as ReceivedEmail,
    );
}

export async function waitForEmailTo(
  recipient: string,
  bodyPattern?: RegExp,
  timeoutMilliseconds = defaultWaitMilliseconds,
): Promise<ReceivedEmail> {
  const deadline = Date.now() + timeoutMilliseconds;
  while (Date.now() < deadline) {
    const email = receivedEmails().find(
      (candidate) =>
        candidate.to.includes(recipient) &&
        (bodyPattern === undefined || bodyPattern.test(candidate.body)),
    );
    if (email !== undefined) {
      return email;
    }
    await new Promise((resolve) => setTimeout(resolve, pollIntervalMilliseconds));
  }
  throw new Error(`No email reached ${recipient} within ${timeoutMilliseconds} ms.`);
}

export async function linkTokenSentTo(recipient: string, linkPattern: RegExp): Promise<string> {
  const email = await waitForEmailTo(recipient, linkPattern);
  const token = linkPattern.exec(email.body)?.[1];
  expect(token, `the email to ${recipient} has the link`).toBeTruthy();
  return decodeURIComponent(token!);
}
