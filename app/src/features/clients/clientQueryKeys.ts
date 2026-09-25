export const clientQueryKeys = {
  all: ["clients"] as const,
  search: (search: string) => [...clientQueryKeys.all, "search", search] as const,
  detail: (clientId: string) => [...clientQueryKeys.all, "detail", clientId] as const,
  appointments: (clientId: string) => [...clientQueryKeys.all, "appointments", clientId] as const,
};
