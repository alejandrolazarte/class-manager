import { getBackofficeJson } from "@/backoffice/api/backofficeHttpClient";

const commandLogPath = "/api/logs/commands";
const securityEventsPath = "/api/logs/security-events";

export const logsQueryKey = ["backofficeLogs"];

export function commandLogQueryKey(day: string) {
  return [...logsQueryKey, "commands", day];
}

export function securityEventsQueryKey(day: string) {
  return [...logsQueryKey, "securityEvents", day];
}

export type CommandOutcome = "Succeeded" | "Failed" | "Exception";

export type CommandSource = "App" | "Backoffice";

export type SecurityEventType =
  "Forbidden" | "PlanLimit" | "SignInFailed" | "RefreshTokenRejected" | "UnauthorizedBurst";

export interface CommandLogEntry {
  occurredAt: string;
  command: string;
  outcome: CommandOutcome;
  errorKind: string | null;
  errorCode: string | null;
  durationMilliseconds: number;
  traceId: string | null;
  payload: string | null;
  payloadTruncated: boolean;
  rowCount: number | null;
  source: CommandSource;
  brandId: string | null;
  brandName: string | null;
  branchName: string | null;
  userId: string | null;
  userName: string | null;
}

export interface CommandLog {
  day: string;
  entries: CommandLogEntry[];
  isTruncated: boolean;
}

export interface SecurityEventEntry {
  occurredAt: string;
  event: SecurityEventType;
  route: string | null;
  method: string | null;
  statusCode: number | null;
  ipAddress: string | null;
  userAgent: string | null;
  count: number | null;
  traceId: string | null;
  brandId: string | null;
  brandName: string | null;
  branchName: string | null;
  userName: string | null;
}

export interface SecurityEventList {
  day: string;
  events: SecurityEventEntry[];
}

export function getCommandLog(day: string): Promise<CommandLog> {
  return getBackofficeJson<CommandLog>(`${commandLogPath}?day=${encodeURIComponent(day)}`);
}

export function getSecurityEvents(day: string): Promise<SecurityEventList> {
  return getBackofficeJson<SecurityEventList>(
    `${securityEventsPath}?day=${encodeURIComponent(day)}`,
  );
}
