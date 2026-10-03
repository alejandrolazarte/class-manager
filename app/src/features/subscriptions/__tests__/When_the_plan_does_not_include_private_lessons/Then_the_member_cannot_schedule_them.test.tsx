import { renderHook } from "@testing-library/react-native";
import { PropsWithChildren } from "react";
import { CurrentMemberProvider, useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { buildCurrentMember } from "@/testing/memberFactory";
import { buildSubscription } from "@/testing/subscriptionFactory";

describe("When the plan does not include private lessons", () => {
  it("Then the member cannot schedule them", async () => {
    const member = buildCurrentMember({ subscription: buildSubscription() });
    const wrapper = ({ children }: PropsWithChildren) => (
      <CurrentMemberProvider member={member}>{children}</CurrentMemberProvider>
    );

    const { result } = await renderHook(
      () => ({
        canSchedule: useCan(permissions.privateLessonsManageAll),
        canSeeStudents: useCan(permissions.studentsViewAll),
      }),
      { wrapper },
    );

    expect(result.current).toEqual({ canSchedule: false, canSeeStudents: true });
  });
});
