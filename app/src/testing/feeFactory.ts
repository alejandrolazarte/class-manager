import { ClassPackClient, ClientFee, MonthlyFees } from "@/features/fees/types";

export const feeMonth = "2026-09";

export function buildClientFee(overrides: Partial<ClientFee> = {}): ClientFee {
  return {
    clientId: "0192f0c4-0000-7000-8000-000000000001",
    clientFullName: "Ana Pérez",
    clientPhoneNumber: "+5491122334455",
    studentNames: ["Lucía Pérez", "Tomás Pérez"],
    fee: 12000,
    paid: 5000,
    balance: 7000,
    status: "Partial",
    ...overrides,
  };
}

export function buildClassPackClient(overrides: Partial<ClassPackClient> = {}): ClassPackClient {
  return {
    clientId: "0192f0c4-0000-7000-8000-000000000002",
    clientFullName: "Jorge Díaz",
    clientPhoneNumber: "+5491133445566",
    studentNames: ["Mateo Díaz"],
    availableClasses: 0,
    unpaidClasses: 2,
    ...overrides,
  };
}

export function buildMonthlyFees(
  clients: ClientFee[],
  classPackClients: ClassPackClient[] = [],
): MonthlyFees {
  return {
    month: feeMonth,
    totalDue: clients.reduce((total, client) => total + (client.fee ?? 0), 0),
    totalPaid: clients.reduce((total, client) => total + client.paid, 0),
    clients,
    classPackSales: 0,
    classPackClients,
  };
}
