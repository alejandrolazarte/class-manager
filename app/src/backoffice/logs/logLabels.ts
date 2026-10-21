import { CommandOutcome, CommandSource, SecurityEventType } from "@/backoffice/logs/logsApi";
import { translate, TranslationKey } from "@/i18n/translate";

const inputTypeSuffixes = ["Command", "Query"];
const utcTimeStart = 11;
const utcTimeEnd = 16;

export const commandOutcomes: readonly CommandOutcome[] = ["Succeeded", "Failed", "Exception"];

export const commandSources: readonly CommandSource[] = ["App", "Backoffice"];

export const securityEventTypes: readonly SecurityEventType[] = [
  "Forbidden",
  "PlanLimit",
  "SignInFailed",
  "RefreshTokenRejected",
  "UnauthorizedBurst",
];

const outcomeKeys: Record<CommandOutcome, TranslationKey> = {
  Succeeded: "backoffice.logs.outcome.Succeeded",
  Failed: "backoffice.logs.outcome.Failed",
  Exception: "backoffice.logs.outcome.Exception",
};

const sourceKeys: Record<CommandSource, TranslationKey> = {
  App: "backoffice.logs.source.App",
  Backoffice: "backoffice.logs.source.Backoffice",
};

const eventKeys: Record<SecurityEventType, TranslationKey> = {
  Forbidden: "backoffice.logs.event.Forbidden",
  PlanLimit: "backoffice.logs.event.PlanLimit",
  SignInFailed: "backoffice.logs.event.SignInFailed",
  RefreshTokenRejected: "backoffice.logs.event.RefreshTokenRejected",
  UnauthorizedBurst: "backoffice.logs.event.UnauthorizedBurst",
};

export function commandLabel(command: string): string {
  const suffix = inputTypeSuffixes.find(
    (candidate) => command.endsWith(candidate) && command.length > candidate.length,
  );
  return suffix === undefined ? command : command.slice(0, -suffix.length);
}

export function outcomeLabel(outcome: CommandOutcome): string {
  return translate(outcomeKeys[outcome]);
}

export function sourceLabel(source: CommandSource): string {
  return translate(sourceKeys[source]);
}

export function eventLabel(event: SecurityEventType): string {
  return translate(eventKeys[event]);
}

export function formatUtcTime(isoDateTime: string): string {
  return new Date(isoDateTime).toISOString().slice(utcTimeStart, utcTimeEnd);
}

export function todayUtcIsoDate(now: Date = new Date()): string {
  return now.toISOString().slice(0, utcTimeStart - 1);
}
