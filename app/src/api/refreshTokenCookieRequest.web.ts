const refreshTokenCookieHeaderName = "X-Refresh-Token-Cookie";
const refreshTokenCookieHeaderValue = "true";

export const refreshTokenCookieCredentials: RequestInit = { credentials: "include" };

export const refreshTokenCookieHeaders: Record<string, string> = {
  [refreshTokenCookieHeaderName]: refreshTokenCookieHeaderValue,
};
