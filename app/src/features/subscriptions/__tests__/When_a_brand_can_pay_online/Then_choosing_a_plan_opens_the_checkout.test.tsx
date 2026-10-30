import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { Platform } from "react-native";
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

const checkoutUrl = "https://checkout.stripe.test/session";

describe("When a brand can pay online", () => {
  beforeEach(() => {
    Platform.OS = "web";
    jest.mocked(listPlans).mockResolvedValue(buildPlans());
    jest
      .mocked(getOrganizationSubscription)
      .mockResolvedValue(buildOrganizationSubscription({ selfService: true }));
    jest.mocked(startPlanCheckout).mockResolvedValue({ url: checkoutUrl });
  });

  it("Then choosing a plan opens the checkout", async () => {
    await renderWithProviders(<PlanScreen />, {
      member: buildCurrentMember({ subscription: buildSubscription() }),
    });

    fireEvent.press(
      await screen.findByLabelText(translate("subscriptions.choosePlanNamed", { plan: "Lite" })),
    );

    await waitFor(() => expect(goToBillingPage).toHaveBeenCalledWith(checkoutUrl));
    expect(jest.mocked(startPlanCheckout).mock.calls[0]?.[0]).toBe("lite");
  });
});
