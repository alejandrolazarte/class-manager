const cancelledErrorName = "NotAllowedError";
const previouslyRegisteredErrorCode = "ERROR_AUTHENTICATOR_PREVIOUSLY_REGISTERED";

export function isPasskeyPromptCancelled(error: unknown): boolean {
  return error instanceof Error && error.name === cancelledErrorName;
}

export function isPasskeyAlreadyOnDevice(error: unknown): boolean {
  return (
    error instanceof Error &&
    "code" in error &&
    (error as { code: unknown }).code === previouslyRegisteredErrorCode
  );
}
