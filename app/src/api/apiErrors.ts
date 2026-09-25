import { ProblemDetails } from "@/api/problemDetails";

export class ApiError extends Error {
  constructor(
    readonly status: number,
    readonly problem: ProblemDetails,
  ) {
    super(problem.title ?? `Request failed with status ${status}`);
    this.name = "ApiError";
  }

  hasCode(problemCode: string): boolean {
    return this.problem.code === problemCode;
  }
}

export class NetworkError extends Error {
  constructor(readonly innerError: unknown) {
    super("The server could not be reached");
    this.name = "NetworkError";
  }
}

export function isApiError(error: unknown): error is ApiError {
  return error instanceof ApiError;
}

export function isNetworkError(error: unknown): error is NetworkError {
  return error instanceof NetworkError;
}
