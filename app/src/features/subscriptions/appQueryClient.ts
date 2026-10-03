import { MutationCache, QueryClient } from "@tanstack/react-query";
import { isApiError } from "@/api/apiErrors";
import { memberQueryKeys } from "@/features/members/memberQueryKeys";
import { subscriptionErrorCodes } from "@/features/subscriptions/subscriptionCodes";

const planRefusalCodes: readonly string[] = Object.values(subscriptionErrorCodes);

export function isPlanRefusal(error: unknown): boolean {
  return isApiError(error) && planRefusalCodes.includes(error.problem.code ?? "");
}

export function createAppQueryClient(): QueryClient {
  const queryClient: QueryClient = new QueryClient({
    mutationCache: new MutationCache({
      onError: (error) => {
        if (isPlanRefusal(error)) {
          void queryClient.invalidateQueries({ queryKey: memberQueryKeys.current() });
        }
      },
    }),
  });
  return queryClient;
}
