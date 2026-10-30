import { screen } from "@testing-library/react-native";
import { Platform } from "react-native";
import { everyPermission, permissions } from "@/features/members/permissions";
import { PlanScreen } from "@/features/subscriptions/screens/PlanScreen";
import { getOrganizationSubscription, listPlans } from "@/features/subscriptions/subscriptionsApi";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import {
  buildOrganizationSubscription,
  buildPlans,
  buildSubscription,
} from "@/testing/subscriptionFactory";

jest.mock("@/features/subscriptions/subscriptionsApi");

describe("When a member cannot manage the subscription", () => {
  beforeEach(() => {
    Platform.OS = "web";
    jest.mocked(listPlans).mockResolvedValue(buildPlans());
    jest
      .mocked(getOrganizationSubscription)
      .mockResolvedValue(buildOrganizationSubscription({ selfService: true }));
  });

  it("Then no payment button is shown", async () => {
    await renderWithProviders(<PlanScreen />, {
      member: buildCurrentMember({
        subscription: buildSubscription(),
        permissions: everyPermission.filter(
          (permission) => permission !== permissions.subscriptionManage,
        ),
      }),
    });

    expect(await screen.findByText(translate("subscriptions.priceOnRequest"))).toBeTruthy();
    expect(screen.queryByText(translate("subscriptions.choosePlan"))).toBeNull();
  });
});
