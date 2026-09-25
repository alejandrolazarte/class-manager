import { isNetworkError } from "@/api/httpClient";

export type SubmissionFailure = "network" | "unexpected";

export function toSubmissionFailure(submissionError: unknown): SubmissionFailure {
  return isNetworkError(submissionError) ? "network" : "unexpected";
}
