import { CheckedInvitation, TokenResponse } from "@/features/authentication/types";

export const storedRefreshToken = "stored-refresh-token";

export function buildTokenResponse(overrides: Partial<TokenResponse> = {}): TokenResponse {
  return {
    accessToken: "issued-access-token",
    accessTokenExpiresAt: "2026-09-24T15:15:00Z",
    refreshToken: "issued-refresh-token",
    refreshTokenExpiresAt: "2026-10-24T15:00:00Z",
    ...overrides,
  };
}

export function buildCheckedInvitation(
  overrides: Partial<CheckedInvitation> = {},
): CheckedInvitation {
  return {
    email: "marcos@example.com",
    businessName: "Natación Olas",
    hasAccount: false,
    ...overrides,
  };
}
