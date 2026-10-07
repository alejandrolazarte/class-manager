import { APIRequestContext, expect } from "@playwright/test";
import { randomUUID } from "node:crypto";
import { e2eEnvironment } from "./environment";

export const ownerPassword = "e2e-owner-password";
export const clientFullName = "Ana Pérez";

const clientPhoneNumber = "11 2233-4455";

export interface BusinessAccount {
  email: string;
  password: string;
  accessToken: string;
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

export async function signUpBusiness(request: APIRequestContext): Promise<BusinessAccount> {
  const email = uniqueOwnerEmail();
  const response = await request.post(apiPath("/api/auth/sign-up"), {
    data: {
      ownerFullName: "Dueña E2E",
      email,
      password: ownerPassword,
      businessName: `Negocio E2E ${randomUUID().slice(0, 8)}`,
      timeZoneId: "America/Argentina/Buenos_Aires",
      currencyCode: "ARS",
      defaultCountryCallingCode: "54",
      ownerBirthDate: "1985-06-20",
    },
  });
  expect(response.status()).toBe(201);
  const tokens = (await response.json()) as { accessToken: string };
  return { email, password: ownerPassword, accessToken: tokens.accessToken };
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
