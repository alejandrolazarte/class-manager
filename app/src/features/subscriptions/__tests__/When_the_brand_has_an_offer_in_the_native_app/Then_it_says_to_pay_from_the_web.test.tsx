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

describe("When the brand has an offer in the native app", () => {
  beforeEach(() => {
    Platform.OS = "ios";
    jest.mocked(listPlans).mockResolvedValue(buildPlans());
    jest.mocked(getOrganizationSubscription).mockResolvedValue(
      buildOrganizationSubscription({
        offer: { planCode: "lite", price: 10, currency: "EUR", expiredOn: null },
      }),
    );
  });

  it("Then it says to pay from the web", async () => {
    await renderWithProviders(<PlanScreen />, {
      member: buildCurrentMember({ subscription: buildSubscription() }),
    });

    expect(await screen.findByText(translate("subscriptions.manageOnWeb"))).toBeTruthy();
    expect(screen.getByTestId("offered-plan")).toBeTruthy();
    expect(screen.queryByText(translate("subscriptions.offer.accept"))).toBeNull();
  });
});
