import { anonymousHttpClient, httpClient } from "@/api/httpClient";
import {
  Account,
  AcceptStudentAppInvitationRequest,
  AcceptInvitationRequest,
  CheckInvitationRequest,
  CheckedInvitation,
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
  acceptStudentAppInvitation: "/api/auth/student-app-invitations/accept",
  checkInvitation: "/api/auth/invitations/check",
  checkStudentAppInvitation: "/api/auth/student-app-invitations/check",
  declineInvitation: "/api/auth/invitations/decline",
  declineStudentAppInvitation: "/api/auth/student-app-invitations/decline",
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

export function checkInvitation(request: CheckInvitationRequest): Promise<CheckedInvitation> {
  return anonymousHttpClient.post<CheckedInvitation>(authenticationPaths.checkInvitation, request);
}

export function checkStudentAppInvitation(
  request: CheckInvitationRequest,
): Promise<CheckedInvitation> {
  return anonymousHttpClient.post<CheckedInvitation>(
    authenticationPaths.checkStudentAppInvitation,
    request,
  );
}

export function declineInvitation(request: CheckInvitationRequest): Promise<void> {
  return anonymousHttpClient.post<void>(authenticationPaths.declineInvitation, request);
}

export function declineStudentAppInvitation(request: CheckInvitationRequest): Promise<void> {
  return anonymousHttpClient.post<void>(authenticationPaths.declineStudentAppInvitation, request);
}

export function acceptStudentAppInvitation(
  request: AcceptStudentAppInvitationRequest,
): Promise<TokenResponse> {
  return anonymousHttpClient.post<TokenResponse>(
    authenticationPaths.acceptStudentAppInvitation,
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
