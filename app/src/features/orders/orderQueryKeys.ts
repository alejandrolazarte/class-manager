export const orderQueryKeys = {
  all: ["orders"] as const,
  list: (clientId: string | null, awaitingPickup: boolean) =>
    [...orderQueryKeys.all, "list", clientId, awaitingPickup] as const,
};
