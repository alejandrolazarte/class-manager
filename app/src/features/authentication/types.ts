import { SessionKind } from "@/features/authentication/sessionKind";

export interface SignUpRequest {
  ownerFullName: string;
  email: string;
  businessName: string;
  countryCode: string;
  timeZoneId: string;
  currencyCode: string;
  ownerBirthDate: string;
  acceptsLegalTerms: boolean;
}

export interface CompleteSignUpRequest {
  token: string;
  password: string;
}

export interface SignInRequest {
  email: string;
  password: string;
}

export interface RefreshSessionRequest {
  refreshToken?: string;
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
  birthDate?: string;
  acceptsLegalTerms?: boolean;
}

export interface AcceptStudentAppInvitationRequest {
  token: string;
  fullName: string;
  password: string;
  birthDate?: string;
  acceptsLegalTerms?: boolean;
}

export interface CheckInvitationRequest {
  token: string;
}

export interface GuardianConsentRequest {
  businessName: string;
  studentFullName: string;
  studentEmail: string;
  guardianEmail: string;
  minimumAge: number;
}

export interface CheckedInvitation {
  email: string;
  businessName: string;
  hasAccount: boolean;
  fullName?: string | null;
  birthDate?: string | null;
}

export interface SwitchBranchRequest extends RefreshSessionRequest {
  businessId: string;
  kind?: SessionKind;
}

export interface Account {
  businessId: string;
  businessName: string;
  kind: SessionKind;
  isCurrent: boolean;
}

export interface TokenResponse {
  accessToken: string;
  accessTokenExpiresAt: string;
  refreshToken?: string;
  refreshTokenExpiresAt: string;
}

export interface RefreshTokenStorage {
  read(): Promise<RefreshSessionRequest | null>;
  write(tokens: TokenResponse): Promise<void>;
  clear(): Promise<void>;
}
