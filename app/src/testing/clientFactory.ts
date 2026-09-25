import { Client } from "@/features/clients/types";

export function buildClient(overrides: Partial<Client> = {}): Client {
  return {
    id: "0192f0c4-0000-7000-8000-000000000001",
    fullName: "Ana Pérez",
    phoneNumber: "+5491122334455",
    email: null,
    notes: null,
    createdAt: "2026-09-24T14:05:00Z",
    ...overrides,
  };
}
