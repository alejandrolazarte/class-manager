import { Passkey, PasskeyCreateRequest, PasskeyGetRequest } from "react-native-passkey";
import {
  PasskeyCreationOptions,
  PasskeyRegistrationCredential,
  PasskeyRequestOptions,
  PasskeySignInCredential,
} from "@/features/passkeys/types";

const publicKeyCredentialType = "public-key";

export function isPasskeySupported(): Promise<boolean> {
  return Promise.resolve(Passkey.isSupported());
}

export async function createPasskeyCredential(
  options: PasskeyCreationOptions,
): Promise<PasskeyRegistrationCredential> {
  const result = await Passkey.create(options as PasskeyCreateRequest);
  return {
    id: result.id,
    rawId: result.rawId,
    type: publicKeyCredentialType,
    clientExtensionResults: {},
    response: {
      clientDataJSON: result.response.clientDataJSON,
      attestationObject: result.response.attestationObject,
      transports: result.response.transports,
    },
  };
}

export async function getPasskeyCredential(
  options: PasskeyRequestOptions,
): Promise<PasskeySignInCredential> {
  const result = await Passkey.get(options as PasskeyGetRequest);
  return {
    id: result.id,
    rawId: result.rawId ?? result.id,
    type: publicKeyCredentialType,
    clientExtensionResults: {},
    response: {
      clientDataJSON: result.response.clientDataJSON,
      authenticatorData: result.response.authenticatorData,
      signature: result.response.signature,
      userHandle: result.response.userHandle,
    },
  };
}

export function suggestedPasskeyName(): string {
  return "Android";
}
