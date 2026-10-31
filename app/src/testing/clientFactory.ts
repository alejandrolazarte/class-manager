import { ClientDetails } from "@/features/clients/types";
import { buildStudentPlan } from "@/testing/feeFactory";

export function buildClient(overrides: Partial<ClientDetails> = {}): ClientDetails {
  return {
    id: "0192f0c4-0000-7000-8000-000000000001",
    fullName: "Ana Pérez",
    phoneNumber: "+5491122334455",
    email: null,
    notes: null,
    createdAt: "2026-09-24T14:05:00Z",
    plans: { monthlyDue: 25000, students: [buildStudentPlan()] },
    students: [],
    appAccess: { status: "NotInvited", invitedEmail: null },
    photoUrl: null,
    ...overrides,
  };
}
