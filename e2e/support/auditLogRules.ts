import { AuditLog, CommandRow } from "./auditLog";

const exceptionOutcome = "Exception";
const failedOutcome = "Failed";
const emailAddressPattern = /[^\s"@]+@[^\s"@]+\.[^\s"@]+/;

export interface ExpectedFailure {
  command: string;
  errorCode: string;
  reason: string;
}

export interface AuditLogExpectations {
  expectedFailures: ExpectedFailure[];
  commandsWithoutSession: string[];
  expectedSecurityEvents: string[];
}

export function auditLogViolations(
  auditLog: AuditLog,
  personalData: string[],
  expectations: AuditLogExpectations,
): string[] {
  return [
    ...nothingWasWritten(auditLog),
    ...commandsThatThrew(auditLog.commands),
    ...unexpectedFailures(auditLog.commands, expectations.expectedFailures),
    ...personalDataInPayloads(auditLog.commands, personalData),
    ...commandsWithoutBusinessOrUser(auditLog.commands, expectations.commandsWithoutSession),
    ...unexpectedSecurityEvents(auditLog, expectations.expectedSecurityEvents),
  ];
}

function nothingWasWritten(auditLog: AuditLog): string[] {
  return auditLog.commands.length === 0
    ? [
        "No command reached the audit log: the API is not writing to Azurite (ConnectionStrings:AuditLog).",
      ]
    : [];
}

function commandsThatThrew(commands: CommandRow[]): string[] {
  return commands
    .filter((row) => row.outcome === exceptionOutcome)
    .map((row) => `${row.command} threw ${row.errorKind}.`);
}

function unexpectedFailures(commands: CommandRow[], expectedFailures: ExpectedFailure[]): string[] {
  return commands
    .filter((row) => row.outcome === failedOutcome)
    .filter(
      (row) =>
        !expectedFailures.some(
          (expected) => expected.command === row.command && expected.errorCode === row.errorCode,
        ),
    )
    .map((row) => `${row.command} failed with ${row.errorKind} ${row.errorCode}.`);
}

function personalDataInPayloads(commands: CommandRow[], personalData: string[]): string[] {
  return commands.flatMap((row) => {
    const texts = payloadTexts(row.payload);
    const leaks = personalData.filter((value) => texts.some((text) => text.includes(value)));
    if (texts.some((text) => emailAddressPattern.test(text))) {
      leaks.push("an email address");
    }
    return leaks.map((leak) => `${row.command} logged ${leak} in its payload.`);
  });
}

function commandsWithoutBusinessOrUser(
  commands: CommandRow[],
  commandsWithoutSession: string[],
): string[] {
  return commands
    .filter((row) => !commandsWithoutSession.includes(row.command))
    .filter((row) => row.tenantId === undefined || row.userId === undefined)
    .map((row) => `${row.command} was logged without its business or its user.`);
}

function unexpectedSecurityEvents(auditLog: AuditLog, expectedSecurityEvents: string[]): string[] {
  return auditLog.securityEvents
    .filter((event) => !expectedSecurityEvents.includes(event.event))
    .map(
      (event) =>
        `Security event ${event.event} on ${event.method ?? ""} ${event.route ?? ""} (${event.statusCode ?? "no status"}).`,
    );
}

function payloadTexts(payload: string | undefined): string[] {
  if (payload === undefined) {
    return [];
  }
  const texts = [payload];
  try {
    collectStrings(JSON.parse(payload) as unknown, texts);
  } catch {
    return texts;
  }
  return texts;
}

function collectStrings(value: unknown, texts: string[]): void {
  if (typeof value === "string") {
    texts.push(value);
  } else if (Array.isArray(value)) {
    value.forEach((item) => collectStrings(item, texts));
  } else if (value !== null && typeof value === "object") {
    Object.values(value).forEach((item) => collectStrings(item, texts));
  }
}
