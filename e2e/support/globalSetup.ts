import { mkdirSync, rmSync } from "node:fs";
import { e2eEnvironment } from "./environment";
import { SmtpSink } from "./smtpSink";

export default async function globalSetup(): Promise<() => Promise<void>> {
  rmSync(e2eEnvironment.mailboxDirectory, { recursive: true, force: true });
  mkdirSync(e2eEnvironment.mailboxDirectory, { recursive: true });
  const smtpSink = new SmtpSink(e2eEnvironment.mailboxDirectory);
  await smtpSink.start(e2eEnvironment.smtpPort);
  return () => smtpSink.stop();
}
