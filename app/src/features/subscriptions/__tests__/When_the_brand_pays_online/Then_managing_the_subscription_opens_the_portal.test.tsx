import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { Platform } from "react-native";
import { goToBillingPage } from "@/features/subscriptions/billingRedirect";
import { PlanScreen } from "@/features/subscriptions/screens/PlanScreen";
import {
  getOrganizationSubscription,
  listPlans,
  openBillingPortal,
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

const portalUrl = "https://billing.stripe.test/session";

describe("When the brand pays online", () => {
  beforeEach(() => {
    Platform.OS = "web";
    jest.mocked(listPlans).mockResolvedValue(buildPlans());
    jest.mocked(getOrganizationSubscription).mockResolvedValue(
      buildOrganizationSubscription({
        planCode: "pro",
        price: 39,
        isExternal: true,
        paymentStatus: "Current",
      }),
    );
    jest.mocked(openBillingPortal).mockResolvedValue({ url: portalUrl });
  });

  it("Then managing the subscription opens the portal", async () => {
    await renderWithProviders(<PlanScreen />, {
      member: buildCurrentMember({ subscription: buildSubscription({ planCode: "pro" }) }),
    });

    fireEvent.press(await screen.findByText(translate("subscriptions.manageBilling")));

    await waitFor(() => expect(goToBillingPage).toHaveBeenCalledWith(portalUrl));
    expect(screen.queryByText(translate("subscriptions.choosePlan"))).toBeNull();
  });
});
