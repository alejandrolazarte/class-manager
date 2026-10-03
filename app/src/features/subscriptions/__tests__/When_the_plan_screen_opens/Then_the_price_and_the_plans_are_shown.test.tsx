import { screen } from "@testing-library/react-native";
import { formatMoney } from "@/features/fees/money";
import { PlanScreen } from "@/features/subscriptions/screens/PlanScreen";
import { getOrganizationSubscription, listPlans } from "@/features/subscriptions/subscriptionsApi";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildPlans, buildSubscription } from "@/testing/subscriptionFactory";

jest.mock("@/features/subscriptions/subscriptionsApi");

const agreedPrice = 12;

describe("When the plan screen opens", () => {
  beforeEach(() => {
    jest.mocked(listPlans).mockResolvedValue(buildPlans());
    jest.mocked(getOrganizationSubscription).mockResolvedValue({
      planCode: "lite",
      price: agreedPrice,
      currency: "USD",
      startsOn: "2026-10-01",
      endsOn: null,
      isActive: true,
    });
  });

  it("Then the price and the plans are shown", async () => {
    const subscription = buildSubscription({ planCode: "lite", endsOn: null });

    await renderWithProviders(<PlanScreen />, { member: buildCurrentMember({ subscription }) });

    expect(
      await screen.findByText(
        translate("subscriptions.pricePerMonth", { price: formatMoney(agreedPrice, "USD") }),
      ),
    ).toBeTruthy();
    expect(await screen.findByText(translate("subscriptions.priceOnRequest"))).toBeTruthy();
  });
});
