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

const litePrice = 9;

describe("When a brand can pay online", () => {
  beforeEach(() => {
    Platform.OS = "web";
    jest.mocked(listPlans).mockResolvedValue(buildPlans());
    jest
      .mocked(getOrganizationSubscription)
      .mockResolvedValue(buildOrganizationSubscription({ selfService: true }));
  });

  it("Then prices are shown plus VAT", async () => {
    await renderWithProviders(<PlanScreen />, {
      member: buildCurrentMember({ subscription: buildSubscription() }),
    });

    expect(
      await screen.findByText(
        translate("subscriptions.pricePerMonthPlusVat", { price: formatMoney(litePrice, "USD") }),
      ),
    ).toBeTruthy();
  });
});
