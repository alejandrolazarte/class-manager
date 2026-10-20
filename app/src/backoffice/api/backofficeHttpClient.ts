import { backofficeConfiguration } from "@/backoffice/backofficeConfiguration";
import { refreshBackofficeSession } from "@/backoffice/session/backofficeSessionApi";

const unauthorizedStatus = 401;

let currentAccessToken: string | null = null;
let pendingRefresh: Promise<string | null> | null = null;

export class BackofficeSessionEndedError extends Error {
  constructor() {
    super("The backoffice session ended.");
    this.name = "BackofficeSessionEndedError";
  }
}

export class BackofficeRequestError extends Error {
  constructor(readonly status: number) {
    super(`The backoffice API answered ${status}.`);
    this.name = "BackofficeRequestError";
  }
}

export function isBackofficeSessionEnded(error: unknown): boolean {
  return error instanceof BackofficeSessionEndedError;
}

export function isBackofficeNotFound(error: unknown): boolean {
  return error instanceof BackofficeRequestError && error.status === 404;
}

export function rememberAccessToken(accessToken: string | null): void {
  currentAccessToken = accessToken;
}

async function renewAccessToken(): Promise<string> {
  pendingRefresh ??= refreshBackofficeSession().finally(() => {
    pendingRefresh = null;
  });
  currentAccessToken = await pendingRefresh;
  if (currentAccessToken === null) {
    throw new BackofficeSessionEndedError();
  }
  return currentAccessToken;
}

function getWithToken(path: string, accessToken: string): Promise<Response> {
  return fetch(backofficeConfiguration.apiBaseUrl + path, {
    headers: { Authorization: `Bearer ${accessToken}` },
  });
}

export async function getBackofficeJson<TResponse>(path: string): Promise<TResponse> {
  let response = await getWithToken(path, currentAccessToken ?? (await renewAccessToken()));
  if (response.status === unauthorizedStatus) {
    response = await getWithToken(path, await renewAccessToken());
  }
  if (!response.ok) {
    throw new BackofficeRequestError(response.status);
  }
  return (await response.json()) as TResponse;
}
