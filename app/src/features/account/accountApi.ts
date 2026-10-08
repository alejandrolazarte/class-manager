import { anonymousHttpClient, httpClient } from "@/api/httpClient";
import {
  ConfirmEmailChangeResponse,
  MyAccount,
  RequestEmailChangeRequest,
  RequestEmailChangeResponse,
  UpdateMyProfileRequest,
} from "@/features/account/types";

const accountPaths = {
  myAccount: "/api/me/account",
  emailChange: "/api/me/account/email-change",
  confirmEmailChange: "/api/auth/email-change/confirm",
} as const;

export function getMyAccount(): Promise<MyAccount> {
  return httpClient.get<MyAccount>(accountPaths.myAccount);
}

export function updateMyProfile(request: UpdateMyProfileRequest): Promise<MyAccount> {
  return httpClient.put<MyAccount>(accountPaths.myAccount, request);
}

export function requestEmailChange(
  request: RequestEmailChangeRequest,
): Promise<RequestEmailChangeResponse> {
  return httpClient.post<RequestEmailChangeResponse>(accountPaths.emailChange, request);
}

export function confirmEmailChange(token: string): Promise<ConfirmEmailChangeResponse> {
  return anonymousHttpClient.post<ConfirmEmailChangeResponse>(accountPaths.confirmEmailChange, {
    token,
  });
}
