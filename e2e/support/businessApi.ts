import { APIRequestContext, expect } from "@playwright/test";
import { randomUUID } from "node:crypto";
import { e2eEnvironment } from "./environment";
import { linkTokenSentTo } from "./mailbox";

export const ownerPassword = "e2e-owner-password";
export const clientFullName = "Ana Pérez";

const clientPhoneNumber = "11 2233-4455";
const completeSignUpLinkPattern = /complete-sign-up\?token=([^\s]+)/;

export interface BusinessAccount {
  email: string;
  password: string;
  accessToken: string;
}

export interface OwnerSignUp {
  ownerFullName: string;
  email: string;
  businessName: string;
  timeZoneId: string;
  currencyCode: string;
  countryCode: string;
  ownerBirthDate: string;
  acceptsLegalTerms: boolean;
}

export interface SeededBusiness extends BusinessAccount {
  clientId: string;
}

interface Identified {
  id: string;
}

function apiPath(path: string): string {
  return `${e2eEnvironment.apiUrl}${path}`;
}

export function uniqueOwnerEmail(): string {
  return `owner-${randomUUID()}@e2e.local`;
}

export async function signUpOwner(
  request: APIRequestContext,
  signUp: OwnerSignUp,
  password: string,
): Promise<BusinessAccount> {
  const requested = await request.post(apiPath("/api/auth/sign-up"), { data: signUp });
  expect(requested.status()).toBe(202);
  const token = await linkTokenSentTo(signUp.email, completeSignUpLinkPattern);
  const completed = await request.post(apiPath("/api/auth/sign-up/complete"), {
    data: { token, password },
  });
  expect(completed.status()).toBe(201);
  const tokens = (await completed.json()) as { accessToken: string };
  return { email: signUp.email, password, accessToken: tokens.accessToken };
}

export async function signUpBusiness(request: APIRequestContext): Promise<BusinessAccount> {
  return signUpOwner(
    request,
    {
      ownerFullName: "Dueña E2E",
      email: uniqueOwnerEmail(),
      businessName: `Negocio E2E ${randomUUID().slice(0, 8)}`,
      timeZoneId: "America/Argentina/Buenos_Aires",
      currencyCode: "ARS",
      countryCode: "AR",
      ownerBirthDate: "1985-06-20",
      acceptsLegalTerms: true,
    },
    ownerPassword,
  );
}

export async function postAsOwner<TResponse>(
  request: APIRequestContext,
  account: BusinessAccount,
  path: string,
  data: unknown,
): Promise<TResponse> {
  const response = await request.post(apiPath(path), {
    data,
    headers: { Authorization: `Bearer ${account.accessToken}` },
  });
  expect(response.ok(), `${path} answered ${response.status()}`).toBeTruthy();
  return (await response.json()) as TResponse;
}

export async function seedBusiness(request: APIRequestContext): Promise<SeededBusiness> {
  const account = await signUpBusiness(request);
  const client = await postAsOwner<Identified>(request, account, "/api/clients", {
    fullName: clientFullName,
    phoneNumber: clientPhoneNumber,
  });
  return { ...account, clientId: client.id };
}
