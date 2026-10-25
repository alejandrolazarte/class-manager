import {
  browserSupportsPasskeys,
  startAuthentication,
  startRegistration,
} from "@simplewebauthn/browser";
import {
  PasskeyCreationOptions,
  PasskeyRegistrationCredential,
  PasskeyRequestOptions,
  PasskeySignInCredential,
} from "@/features/passkeys/types";

const deviceNamesByUserAgentPattern: readonly (readonly [RegExp, string])[] = [
  [/iPhone/, "iPhone"],
  [/iPad/, "iPad"],
  [/Android/, "Android"],
  [/Macintosh|Mac OS X/, "Mac"],
  [/Windows/, "Windows"],
  [/CrOS/, "Chromebook"],
  [/Linux/, "Linux"],
];

export function isPasskeySupported(): Promise<boolean> {
  return browserSupportsPasskeys().catch(() => false);
}

export function createPasskeyCredential(
  options: PasskeyCreationOptions,
): Promise<PasskeyRegistrationCredential> {
  return startRegistration({ optionsJSON: options });
}

export function getPasskeyCredential(
  options: PasskeyRequestOptions,
): Promise<PasskeySignInCredential> {
  return startAuthentication({ optionsJSON: options });
}

export function suggestedPasskeyName(): string {
  const userAgent = globalThis.navigator?.userAgent ?? "";
  const match = deviceNamesByUserAgentPattern.find(([pattern]) => pattern.test(userAgent));
  return match ? match[1] : "";
}
