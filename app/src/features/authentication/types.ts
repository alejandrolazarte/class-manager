import { SessionKind } from "@/features/authentication/sessionKind";

export interface SignUpRequest {
  ownerFullName: string;
  email: string;
  password: string;
  businessName: string;
  timeZoneId: string;
  currencyCode: string;
  defaultCountryCallingCode: string;
}

export interface SignInRequest {
  email: string;
  password: string;
}

export interface RefreshSessionRequest {
  refreshToken: string;
}

export interface PasswordResetRequest {
  email: string;
}

export interface ResetPasswordRequest {
  token: string;
  newPassword: string;
}

export interface AcceptInvitationRequest {
  token: string;
  fullName: string;
  password: string;
}

export interface AcceptStudentAppInvitationRequest {
  token: string;
  fullName: string;
  password: string;
}

export interface CheckInvitationRequest {
  token: string;
}

export interface CheckedInvitation {
  email: string;
  businessName: string;
  hasAccount: boolean;
}

export interface SwitchBranchRequest {
  refreshToken: string;
  businessId: string;
  kind?: SessionKind;
}

export interface Account {
  businessId: string;
  businessName: string;
  kind: SessionKind;
  isCurrent: boolean;
}

export interface SignOutRequest {
  refreshToken: string;
}

export interface TokenResponse {
  accessToken: string;
  accessTokenExpiresAt: string;
  refreshToken: string;
  refreshTokenExpiresAt: string;
}

export interface RefreshTokenStorage {
  read(): Promise<string | null>;
  write(refreshToken: string): Promise<void>;
  clear(): Promise<void>;
}
