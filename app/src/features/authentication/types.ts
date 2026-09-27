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

export interface ResetPasswordRequest {
  token: string;
  newPassword: string;
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
