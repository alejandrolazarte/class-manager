import { CommandLogEntry, SecurityEventEntry } from "@/backoffice/logs/logsApi";

export function aCommandLogEntry(overrides: Partial<CommandLogEntry> = {}): CommandLogEntry {
  return {
    occurredAt: "2026-10-21T14:32:00Z",
    command: "RecordPaymentCommand",
    outcome: "Succeeded",
    errorKind: null,
    errorCode: null,
    durationMilliseconds: 18,
    traceId: "trace-1",
    payload: '{"ClientId":"client-1","Amount":25000}',
    payloadTruncated: false,
    rowCount: null,
    source: "App",
    brandId: "brand-1",
    brandName: "Estudio Centro",
    branchName: "Sede Norte",
    userId: "user-1",
    userName: "Ana Pérez",
    ...overrides,
  };
}

export function aSecurityEventEntry(
  overrides: Partial<SecurityEventEntry> = {},
): SecurityEventEntry {
  return {
    occurredAt: "2026-10-21T09:15:00Z",
    event: "SignInFailed",
    route: "/api/auth/signin",
    method: "POST",
    statusCode: 401,
    ipAddress: "203.0.113.7",
    userAgent: "Mozilla/5.0",
    count: null,
    traceId: "trace-2",
    brandId: null,
    brandName: null,
    branchName: null,
    userName: null,
    ...overrides,
  };
}
