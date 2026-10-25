import {
  Passkey,
  PasskeyCeremony,
  PasskeyCreationOptions,
  PasskeyRegistrationCredential,
  PasskeyRequestOptions,
  PasskeySignInCredential,
} from "@/features/passkeys/types";

export function buildPasskey(overrides: Partial<Passkey> = {}): Passkey {
  return {
    id: "credential-iphone",
    name: "iPhone",
    createdAt: "2026-10-20T15:00:00+00:00",
    ...overrides,
  };
}

export function buildPasskeySignInCeremony(): PasskeyCeremony<PasskeyRequestOptions> {
  return { ceremonyId: "sign-in-ceremony", options: { challenge: "sign-in-challenge" } };
}

export function buildPasskeyRegistrationCeremony(): PasskeyCeremony<PasskeyCreationOptions> {
  return {
    ceremonyId: "registration-ceremony",
    options: {
      challenge: "registration-challenge",
      rp: { name: "Laztty", id: "localhost" },
      user: { id: "dXNlcg", name: "laura@example.com", displayName: "Laura Gómez" },
      pubKeyCredParams: [{ type: "public-key", alg: -7 }],
    },
  };
}

export function buildPasskeySignInCredential(): PasskeySignInCredential {
  return {
    id: "credential-iphone",
    rawId: "credential-iphone",
    type: "public-key",
    clientExtensionResults: {},
    response: {
      clientDataJSON: "client-data",
      authenticatorData: "authenticator-data",
      signature: "signature",
    },
  };
}

export function buildPasskeyRegistrationCredential(): PasskeyRegistrationCredential {
  return {
    id: "credential-iphone",
    rawId: "credential-iphone",
    type: "public-key",
    clientExtensionResults: {},
    response: { clientDataJSON: "client-data", attestationObject: "attestation-object" },
  };
}

export function passkeyPromptCancelledError(): Error {
  const cancelledError = new Error("The operation either timed out or was not allowed.");
  cancelledError.name = "NotAllowedError";
  return cancelledError;
}
