import { anonymousHttpClient, httpClient } from "@/api/httpClient";
import { TokenResponse } from "@/features/authentication/types";
import {
  FinishPasskeyRegistrationRequest,
  Passkey,
  PasskeyCeremony,
  PasskeyCreationOptions,
  PasskeyRequestOptions,
  SignInWithPasskeyRequest,
} from "@/features/passkeys/types";

const passkeyPaths = {
  myPasskeys: "/api/me/passkeys",
  registrationOptions: "/api/me/passkeys/registration-options",
  signInOptions: "/api/auth/passkey/sign-in-options",
  signIn: "/api/auth/passkey/sign-in",
} as const;

export function beginPasskeyRegistration(
  currentPassword: string,
): Promise<PasskeyCeremony<PasskeyCreationOptions>> {
  return httpClient.post<PasskeyCeremony<PasskeyCreationOptions>>(
    passkeyPaths.registrationOptions,
    { currentPassword },
  );
}

export function finishPasskeyRegistration(
  request: FinishPasskeyRegistrationRequest,
): Promise<Passkey> {
  return httpClient.post<Passkey>(passkeyPaths.myPasskeys, request);
}

export function listMyPasskeys(): Promise<Passkey[]> {
  return httpClient.get<Passkey[]>(passkeyPaths.myPasskeys);
}

export function removeMyPasskey(passkeyId: string): Promise<void> {
  return httpClient.delete<void>(`${passkeyPaths.myPasskeys}/${encodeURIComponent(passkeyId)}`);
}

export function beginPasskeySignIn(): Promise<PasskeyCeremony<PasskeyRequestOptions>> {
  return anonymousHttpClient.post<PasskeyCeremony<PasskeyRequestOptions>>(
    passkeyPaths.signInOptions,
    {},
  );
}

export function signInWithPasskey(request: SignInWithPasskeyRequest): Promise<TokenResponse> {
  return anonymousHttpClient.post<TokenResponse>(passkeyPaths.signIn, request);
}
