import { OperatorList } from "@/backoffice/operators/operatorsApi";

export function anOperatorList(overrides: Partial<OperatorList> = {}): OperatorList {
  return {
    operators: [
      {
        loginId: "login-1",
        provider: "google",
        email: "operadora@example.com",
        fullName: "Operadora Uno",
        role: "PlatformOperator",
        branchName: "Laztty",
        enabledAt: "2026-10-01T10:00:00Z",
        isCurrentOperator: true,
      },
      {
        loginId: "login-2",
        provider: "google",
        email: "lectora@example.com",
        fullName: "Lectora Dos",
        role: "PlatformReader",
        branchName: "Laztty",
        enabledAt: "2026-10-05T10:00:00Z",
        isCurrentOperator: false,
      },
    ],
    accessRequests: [
      {
        id: "request-1",
        provider: "google",
        email: "nueva@example.com",
        code: "K7-3QF",
        requestedAt: "2026-10-20T09:00:00Z",
      },
    ],
    teamMembers: [
      {
        memberId: "member-1",
        fullName: "Persona Nueva",
        email: "nueva@example.com",
        branchName: "Laztty",
        role: "Viewer",
      },
    ],
    ...overrides,
  };
}
