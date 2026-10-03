import { MutationObserver } from "@tanstack/react-query";
import { ApiError } from "@/api/apiErrors";
import { memberQueryKeys } from "@/features/members/memberQueryKeys";
import { createAppQueryClient } from "@/features/subscriptions/appQueryClient";

const forbiddenStatus = 403;

describe("When a change is refused by the plan", () => {
  it("Then the member access is reloaded", async () => {
    const queryClient = createAppQueryClient();
    const invalidateQueries = jest.spyOn(queryClient, "invalidateQueries");
    const observer = new MutationObserver(queryClient, {
      mutationFn: () =>
        Promise.reject(new ApiError(forbiddenStatus, { code: "subscription.inactive" })),
    });

    await observer.mutate().catch(() => undefined);

    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: memberQueryKeys.current() });
    queryClient.clear();
  });
});
