import { odata, TableClient, TableServiceClient } from "@azure/data-tables";
import { BlobServiceClient } from "@azure/storage-blob";
import { e2eEnvironment } from "./environment";

export const auditLogRunStartedAtVariable = "E2E_AUDIT_LOG_RUN_STARTED_AT";

const commandTablePrefix = "Commands";
const securityEventsContainer = "security-events";
const noBusinessPartition = "none";
const settleIntervalMilliseconds = 1_000;
const settleLimitMilliseconds = 15_000;
const millisecondsPerDay = 86_400_000;

export interface CommandRow {
  tenantId: string | undefined;
  command: string;
  userId: string | undefined;
  outcome: string;
  errorKind: string | undefined;
  errorCode: string | undefined;
  payload: string | undefined;
}

export interface SecurityEventLine {
  occurredAt: string;
  event: string;
  route?: string;
  method?: string;
  statusCode?: number;
}

export interface AuditLog {
  commands: CommandRow[];
  securityEvents: SecurityEventLine[];
}

interface StoredCommandRow {
  partitionKey: string;
  Command: string;
  UserId?: string;
  Outcome: string;
  ErrorKind?: string;
  ErrorCode?: string;
  Payload?: string;
}

export function auditLogRunStartedAt(): Date {
  const startedAt = process.env[auditLogRunStartedAtVariable];
  if (startedAt === undefined) {
    throw new Error(`${auditLogRunStartedAtVariable} is set by the global setup.`);
  }
  return new Date(startedAt);
}

export async function listCommandTables(): Promise<string[]> {
  const tables: string[] = [];
  const serviceClient = TableServiceClient.fromConnectionString(
    e2eEnvironment.auditLogConnectionString,
    { allowInsecureConnection: true },
  );
  for await (const table of serviceClient.listTables()) {
    if (table.name !== undefined) {
      tables.push(table.name);
    }
  }
  return tables;
}

export async function readAuditLogOnceSettled(since: Date): Promise<AuditLog> {
  const deadline = Date.now() + settleLimitMilliseconds;
  let previous = await readAuditLog(since);
  while (Date.now() < deadline) {
    await new Promise((resolve) => setTimeout(resolve, settleIntervalMilliseconds));
    const current = await readAuditLog(since);
    if (
      current.commands.length === previous.commands.length &&
      current.securityEvents.length === previous.securityEvents.length
    ) {
      return current;
    }
    previous = current;
  }
  return previous;
}

async function readAuditLog(since: Date): Promise<AuditLog> {
  return {
    commands: await readCommands(since),
    securityEvents: await readSecurityEvents(since),
  };
}

async function readCommands(since: Date): Promise<CommandRow[]> {
  const rows: CommandRow[] = [];
  const existingTables = new Set(await listCommandTables());
  for (const tableName of monthTablesSince(since).filter((name) => existingTables.has(name))) {
    const table = TableClient.fromConnectionString(
      e2eEnvironment.auditLogConnectionString,
      tableName,
      { allowInsecureConnection: true },
    );
    const entities = table.listEntities<StoredCommandRow>({
      queryOptions: { filter: odata`OccurredAt ge ${since}` },
    });
    for await (const entity of entities) {
      rows.push({
        tenantId: entity.partitionKey === noBusinessPartition ? undefined : entity.partitionKey,
        command: entity.Command,
        userId: entity.UserId,
        outcome: entity.Outcome,
        errorKind: entity.ErrorKind,
        errorCode: entity.ErrorCode,
        payload: entity.Payload,
      });
    }
  }
  return rows;
}

async function readSecurityEvents(since: Date): Promise<SecurityEventLine[]> {
  const container = BlobServiceClient.fromConnectionString(
    e2eEnvironment.auditLogConnectionString,
  ).getContainerClient(securityEventsContainer);
  if (!(await container.exists())) {
    return [];
  }

  const events: SecurityEventLine[] = [];
  for (const blobName of dayBlobsSince(since)) {
    const blob = container.getBlobClient(blobName);
    if (!(await blob.exists())) {
      continue;
    }
    const content = (await blob.downloadToBuffer()).toString("utf8");
    events.push(
      ...content
        .split("\n")
        .filter((line) => line !== "")
        .map((line) => JSON.parse(line) as SecurityEventLine)
        .filter((event) => new Date(event.occurredAt) >= since),
    );
  }
  return events;
}

function monthTablesSince(since: Date): string[] {
  const tables: string[] = [];
  const now = new Date();
  for (
    let month = new Date(Date.UTC(since.getUTCFullYear(), since.getUTCMonth(), 1));
    month <= now;
    month = new Date(Date.UTC(month.getUTCFullYear(), month.getUTCMonth() + 1, 1))
  ) {
    tables.push(
      `${commandTablePrefix}${month.getUTCFullYear()}${twoDigits(month.getUTCMonth() + 1)}`,
    );
  }
  return tables;
}

function dayBlobsSince(since: Date): string[] {
  const blobs: string[] = [];
  const now = new Date();
  for (
    let day = new Date(Date.UTC(since.getUTCFullYear(), since.getUTCMonth(), since.getUTCDate()));
    day <= now;
    day = new Date(day.getTime() + millisecondsPerDay)
  ) {
    blobs.push(
      `${day.getUTCFullYear()}/${twoDigits(day.getUTCMonth() + 1)}/${twoDigits(day.getUTCDate())}.jsonl`,
    );
  }
  return blobs;
}

function twoDigits(value: number): string {
  return String(value).padStart(2, "0");
}
