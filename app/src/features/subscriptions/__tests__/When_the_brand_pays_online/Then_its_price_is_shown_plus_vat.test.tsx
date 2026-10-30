import { screen } from "@testing-library/react-native";
import { Platform } from "react-native";
import { formatMoney } from "@/formatting/money";
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

const agreedPrice = 29;

describe("When the brand pays online", () => {
  beforeEach(() => {
    Platform.OS = "web";
    jest.mocked(listPlans).mockResolvedValue(buildPlans());
    jest.mocked(getOrganizationSubscription).mockResolvedValue(
      buildOrganizationSubscription({
        planCode: "pro",
        price: agreedPrice,
        isExternal: true,
        paymentStatus: "Current",
      }),
    );
  });

  it("Then its price is shown plus VAT", async () => {
    await renderWithProviders(<PlanScreen />, {
      member: buildCurrentMember({ subscription: buildSubscription({ planCode: "pro" }) }),
    });

    expect(
      await screen.findByText(
        translate("subscriptions.pricePerMonthPlusVat", { price: formatMoney(agreedPrice, "EUR") }),
      ),
    ).toBeTruthy();
  });
});
