export const feeQueryKeys = {
  all: ["fees"] as const,
  month: (month: string) => [...feeQueryKeys.all, "month", month] as const,
  clientPayments: (clientId: string) => [...feeQueryKeys.all, "payments", clientId] as const,
};
