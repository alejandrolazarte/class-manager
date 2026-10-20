import {
  refreshTokenCookieCredentials,
  refreshTokenCookieHeaders,
} from "@/api/refreshTokenCookieRequest";
import { backofficeConfiguration } from "@/backoffice/backofficeConfiguration";

const googleSignInPath = "/api/auth/signin/google";
const refreshPath = "/api/auth/refresh";
const signOutPath = "/api/auth/signout";
const currentOperatorPath = "/api/me";
const unauthorizedStatus = 401;
const postMethod = "POST";

export interface CurrentOperator {
  email: string;
  role: string;
}

interface AccessTokenResponse {
  accessToken: string;
}

function urlOf(path: string): string {
  return backofficeConfiguration.apiBaseUrl + path;
}

export function googleSignInUrl(): string {
  return urlOf(googleSignInPath);
}

export async function refreshBackofficeSession(): Promise<string | null> {
  const response = await fetch(urlOf(refreshPath), {
    ...refreshTokenCookieCredentials,
    method: postMethod,
    headers: refreshTokenCookieHeaders,
  });
  if (response.status === unauthorizedStatus) {
    return null;
  }
  if (!response.ok) {
    throw new Error(`Refreshing the backoffice session answered ${response.status}.`);
  }
  return ((await response.json()) as AccessTokenResponse).accessToken;
}

export async function getCurrentOperator(accessToken: string): Promise<CurrentOperator> {
  const response = await fetch(urlOf(currentOperatorPath), {
    headers: { Authorization: `Bearer ${accessToken}` },
  });
  if (!response.ok) {
    throw new Error(`Reading the current operator answered ${response.status}.`);
  }
  return (await response.json()) as CurrentOperator;
}

export async function signOutOfBackoffice(): Promise<void> {
  await fetch(urlOf(signOutPath), {
    ...refreshTokenCookieCredentials,
    method: postMethod,
    headers: refreshTokenCookieHeaders,
  });
}
