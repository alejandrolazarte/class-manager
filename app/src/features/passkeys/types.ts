import type {
  AuthenticationResponseJSON,
  PublicKeyCredentialCreationOptionsJSON,
  PublicKeyCredentialRequestOptionsJSON,
  RegistrationResponseJSON,
} from "@simplewebauthn/browser";

export type PasskeyCreationOptions = PublicKeyCredentialCreationOptionsJSON;

export type PasskeyRequestOptions = PublicKeyCredentialRequestOptionsJSON;

export type PasskeyRegistrationCredential = RegistrationResponseJSON;

export type PasskeySignInCredential = AuthenticationResponseJSON;

export interface PasskeyCeremony<TOptions> {
  ceremonyId: string;
  options: TOptions;
}

export interface Passkey {
  id: string;
  name: string;
  createdAt: string;
}

export interface FinishPasskeyRegistrationRequest {
  ceremonyId: string;
  credential: PasskeyRegistrationCredential;
  name: string;
}

export interface SignInWithPasskeyRequest {
  ceremonyId: string;
  credential: PasskeySignInCredential;
}
