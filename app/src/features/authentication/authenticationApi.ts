import { anonymousHttpClient, httpClient } from "@/api/httpClient";
import {
  Account,
  AcceptFamilyInvitationRequest,
  AcceptInvitationRequest,
  CheckInvitationRequest,
  PasswordResetRequest,
  RefreshSessionRequest,
  ResetPasswordRequest,
  SignInRequest,
  SignOutRequest,
  SignUpRequest,
  SwitchBranchRequest,
  TokenResponse,
} from "@/features/authentication/types";

const authenticationPaths = {
  signUp: "/api/auth/sign-up",
  signIn: "/api/auth/sign-in",
  refresh: "/api/auth/refresh",
  signOut: "/api/auth/sign-out",
  passwordReset: "/api/auth/password-reset",
  passwordResetRequest: "/api/auth/password-reset-request",
  acceptInvitation: "/api/auth/invitations/accept",
  acceptFamilyInvitation: "/api/auth/family-invitations/accept",
  checkInvitation: "/api/auth/invitations/check",
  checkFamilyInvitation: "/api/auth/family-invitations/check",
  switchBranch: "/api/auth/branch",
  accounts: "/api/me/accounts",
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

export function resetPassword(request: ResetPasswordRequest): Promise<void> {
  return anonymousHttpClient.post<void>(authenticationPaths.passwordReset, request);
}

export function acceptInvitation(request: AcceptInvitationRequest): Promise<TokenResponse> {
  return anonymousHttpClient.post<TokenResponse>(authenticationPaths.acceptInvitation, request);
}

export function checkInvitation(request: CheckInvitationRequest): Promise<void> {
  return anonymousHttpClient.post<void>(authenticationPaths.checkInvitation, request);
}

export function checkFamilyInvitation(request: CheckInvitationRequest): Promise<void> {
  return anonymousHttpClient.post<void>(authenticationPaths.checkFamilyInvitation, request);
}

export function acceptFamilyInvitation(
  request: AcceptFamilyInvitationRequest,
): Promise<TokenResponse> {
  return anonymousHttpClient.post<TokenResponse>(
    authenticationPaths.acceptFamilyInvitation,
    request,
  );
}

export function switchBranch(request: SwitchBranchRequest): Promise<TokenResponse> {
  return anonymousHttpClient.post<TokenResponse>(authenticationPaths.switchBranch, request);
}

export function listAccounts(): Promise<Account[]> {
  return httpClient.get<Account[]>(authenticationPaths.accounts);
}

export function requestPasswordReset(request: PasswordResetRequest): Promise<void> {
  return anonymousHttpClient.post<void>(authenticationPaths.passwordResetRequest, request);
}
