import { screen } from "@testing-library/react-native";
import { Platform } from "react-native";
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

describe("When the app is native", () => {
  beforeEach(() => {
    Platform.OS = "ios";
    jest.mocked(listPlans).mockResolvedValue(buildPlans());
    jest
      .mocked(getOrganizationSubscription)
      .mockResolvedValue(buildOrganizationSubscription({ selfService: true }));
  });

  it("Then no payment button is shown and the web is suggested", async () => {
    await renderWithProviders(<PlanScreen />, {
      member: buildCurrentMember({ subscription: buildSubscription() }),
    });

    expect(await screen.findByText(translate("subscriptions.manageOnWeb"))).toBeTruthy();
    expect(screen.queryByText(translate("subscriptions.choosePlan"))).toBeNull();
  });
});
