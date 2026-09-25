export const clientQueryKeys = {
  all: ["clients"] as const,
  detail: (clientId: string) => [...clientQueryKeys.all, "detail", clientId] as const,
};
