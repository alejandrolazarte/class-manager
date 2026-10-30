import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { Platform } from "react-native";
import { formatMoney } from "@/formatting/money";
import { goToBillingPage } from "@/features/subscriptions/billingRedirect";
import { PlanScreen } from "@/features/subscriptions/screens/PlanScreen";
import {
  getOrganizationSubscription,
  listPlans,
  startPlanCheckout,
} from "@/features/subscriptions/subscriptionsApi";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import {
  buildOrganizationSubscription,
  buildPlans,
  buildSubscription,
} from "@/testing/subscriptionFactory";

jest.mock("@/features/subscriptions/subscriptionsApi");
jest.mock("@/features/subscriptions/billingRedirect");

const checkoutUrl = "https://checkout.stripe.test/offer";
const offeredPrice = 10;

describe("When the brand has an offer", () => {
  beforeEach(() => {
    Platform.OS = "web";
    jest.mocked(listPlans).mockResolvedValue(buildPlans());
    jest.mocked(getOrganizationSubscription).mockResolvedValue(
      buildOrganizationSubscription({
        offer: { planCode: "lite", price: offeredPrice, currency: "EUR", expiredOn: null },
      }),
    );
    jest.mocked(startPlanCheckout).mockResolvedValue({ url: checkoutUrl });
  });

  it("Then accepting it opens the checkout at the offered price", async () => {
    await renderWithProviders(<PlanScreen />, {
      member: buildCurrentMember({ subscription: buildSubscription() }),
    });

    expect(
      await screen.findByText(
        translate("subscriptions.offer.title", {
          plan: "Lite",
          price: formatMoney(offeredPrice, "EUR"),
        }),
      ),
    ).toBeTruthy();
    fireEvent.press(screen.getByText(translate("subscriptions.offer.accept")));

    await waitFor(() => expect(goToBillingPage).toHaveBeenCalledWith(checkoutUrl));
    expect(jest.mocked(startPlanCheckout).mock.calls[0]?.[0]).toBe("lite");
  });
});
