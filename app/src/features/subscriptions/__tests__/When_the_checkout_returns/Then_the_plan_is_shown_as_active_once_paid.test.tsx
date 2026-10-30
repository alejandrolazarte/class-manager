import { screen } from "@testing-library/react-native";
import { Platform } from "react-native";
import { PlanScreen } from "@/features/subscriptions/screens/PlanScreen";
import { getOrganizationSubscription, listPlans } from "@/features/subscriptions/subscriptionsApi";
import { translate } from "@/i18n/translate";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import {
  buildOrganizationSubscription,
  buildPlans,
  buildSubscription,
} from "@/testing/subscriptionFactory";

jest.mock("@/features/subscriptions/subscriptionsApi");

describe("When the checkout returns", () => {
  beforeEach(() => {
    Platform.OS = "web";
    searchParametersMock.current = { checkout: "success" };
    jest.mocked(listPlans).mockResolvedValue(buildPlans());
    jest.mocked(getOrganizationSubscription).mockResolvedValue(
      buildOrganizationSubscription({
        planCode: "pro",
        isExternal: true,
        paymentStatus: "Current",
      }),
    );
  });

  it("Then the plan is shown as active once paid", async () => {
    await renderWithProviders(<PlanScreen />, {
      member: buildCurrentMember({ subscription: buildSubscription({ planCode: "pro" }) }),
    });

    expect(await screen.findByText(translate("subscriptions.checkoutDone"))).toBeTruthy();
  });
});
