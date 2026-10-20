import { ProblemDetails } from "@/api/problemDetails";
import { backofficeConfiguration } from "@/backoffice/backofficeConfiguration";
import { refreshBackofficeSession } from "@/backoffice/session/backofficeSessionApi";

const unauthorizedStatus = 401;
const notFoundStatus = 404;
const jsonContentType = "application/json";

type HttpMethod = "GET" | "POST" | "PUT" | "DELETE";

let currentAccessToken: string | null = null;
let pendingRefresh: Promise<string | null> | null = null;

export class BackofficeSessionEndedError extends Error {
  constructor() {
    super("The backoffice session ended.");
    this.name = "BackofficeSessionEndedError";
  }
}

export class BackofficeRequestError extends Error {
  constructor(
    readonly status: number,
    readonly problem: ProblemDetails = { status },
  ) {
    super(`The backoffice API answered ${status}.`);
    this.name = "BackofficeRequestError";
  }
}

export function isBackofficeSessionEnded(error: unknown): boolean {
  return error instanceof BackofficeSessionEndedError;
}

export function isBackofficeNotFound(error: unknown): boolean {
  return error instanceof BackofficeRequestError && error.status === notFoundStatus;
}

export function forgetAccessToken(): void {
  currentAccessToken = null;
}

export async function restoreAccessToken(): Promise<string | null> {
  pendingRefresh ??= refreshBackofficeSession().finally(() => {
    pendingRefresh = null;
  });
  currentAccessToken = await pendingRefresh;
  return currentAccessToken;
}

async function renewAccessToken(): Promise<string> {
  const accessToken = await restoreAccessToken();
  if (accessToken === null) {
    throw new BackofficeSessionEndedError();
  }
  return accessToken;
}

function sendWithToken(
  method: HttpMethod,
  path: string,
  accessToken: string,
  body: unknown,
): Promise<Response> {
  const hasBody = body !== undefined;
  return fetch(backofficeConfiguration.apiBaseUrl + path, {
    method,
    headers: hasBody
      ? { Authorization: `Bearer ${accessToken}`, "Content-Type": jsonContentType }
      : { Authorization: `Bearer ${accessToken}` },
    body: hasBody ? JSON.stringify(body) : undefined,
  });
}

async function readProblem(response: Response): Promise<ProblemDetails> {
  try {
    return { ...((await response.json()) as ProblemDetails), status: response.status };
  } catch {
    return { status: response.status };
  }
}

export async function sendBackofficeJson<TResponse>(
  method: HttpMethod,
  path: string,
  body?: unknown,
): Promise<TResponse> {
  let response = await sendWithToken(
    method,
    path,
    currentAccessToken ?? (await renewAccessToken()),
    body,
  );
  if (response.status === unauthorizedStatus) {
    response = await sendWithToken(method, path, await renewAccessToken(), body);
  }
  if (!response.ok) {
    throw new BackofficeRequestError(response.status, await readProblem(response));
  }
  return (await response.json()) as TResponse;
}

export function getBackofficeJson<TResponse>(path: string): Promise<TResponse> {
  return sendBackofficeJson<TResponse>("GET", path);
}
