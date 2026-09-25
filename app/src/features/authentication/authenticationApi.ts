import { anonymousHttpClient } from "@/api/httpClient";
import {
  RefreshSessionRequest,
  SignInRequest,
  SignOutRequest,
  SignUpRequest,
  TokenResponse,
} from "@/features/authentication/types";

const authenticationPaths = {
  signUp: "/api/auth/sign-up",
  signIn: "/api/auth/sign-in",
  refresh: "/api/auth/refresh",
  signOut: "/api/auth/sign-out",
} as const;

export function signUp(request: SignUpRequest): Promise<TokenResponse> {
  return anonymousHttpClient.post<TokenResponse>(authenticationPaths.signUp, request);
}

export function signIn(request: SignInRequest): Promise<TokenResponse> {
  return anonymousHttpClient.post<TokenResponse>(authenticationPaths.signIn, request);
}

export function refreshSession(request: RefreshSessionRequest): Promise<TokenResponse> {
  return anonymousHttpClient.post<TokenResponse>(authenticationPaths.refresh, request);
}

export function signOut(request: SignOutRequest): Promise<void> {
  return anonymousHttpClient.post<void>(authenticationPaths.signOut, request);
}
