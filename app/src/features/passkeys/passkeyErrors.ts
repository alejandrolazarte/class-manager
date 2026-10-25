const cancelledErrorName = "NotAllowedError";
const previouslyRegisteredErrorCode = "ERROR_AUTHENTICATOR_PREVIOUSLY_REGISTERED";
const nativeCancelledError = "UserCancelled";
const nativeAlreadyExistsError = "CredentialAlreadyExists";

function nativeErrorOf(error: unknown): unknown {
  return typeof error === "object" && error !== null && "error" in error
    ? (error as { error: unknown }).error
    : undefined;
}

export function isPasskeyPromptCancelled(error: unknown): boolean {
  return (
    (error instanceof Error && error.name === cancelledErrorName) ||
    nativeErrorOf(error) === nativeCancelledError
  );
}

export function isPasskeyAlreadyOnDevice(error: unknown): boolean {
  return (
    (error instanceof Error &&
      "code" in error &&
      (error as { code: unknown }).code === previouslyRegisteredErrorCode) ||
    nativeErrorOf(error) === nativeAlreadyExistsError
  );
}
