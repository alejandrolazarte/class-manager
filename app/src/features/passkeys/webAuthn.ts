import {
  PasskeyCreationOptions,
  PasskeyRegistrationCredential,
  PasskeyRequestOptions,
  PasskeySignInCredential,
} from "@/features/passkeys/types";

class PasskeysUnsupportedError extends Error {
  constructor() {
    super("Passkeys are only supported on the web for now");
    this.name = "PasskeysUnsupportedError";
  }
}

export function isPasskeySupported(): Promise<boolean> {
  return Promise.resolve(false);
}

export function createPasskeyCredential(
  _options: PasskeyCreationOptions,
): Promise<PasskeyRegistrationCredential> {
  return Promise.reject(new PasskeysUnsupportedError());
}

export function getPasskeyCredential(
  _options: PasskeyRequestOptions,
): Promise<PasskeySignInCredential> {
  return Promise.reject(new PasskeysUnsupportedError());
}

export function suggestedPasskeyName(): string {
  return "";
}
