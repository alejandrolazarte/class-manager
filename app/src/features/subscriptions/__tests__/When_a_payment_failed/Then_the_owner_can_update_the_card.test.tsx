import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { Platform } from "react-native";
import { goToBillingPage } from "@/features/subscriptions/billingRedirect";
import { SubscriptionNotice } from "@/features/subscriptions/components/SubscriptionNotice";
import {
  getOrganizationSubscription,
  openBillingPortal,
} from "@/features/subscriptions/subscriptionsApi";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildOrganizationSubscription, buildSubscription } from "@/testing/subscriptionFactory";

jest.mock("@/features/subscriptions/subscriptionsApi");
jest.mock("@/features/subscriptions/billingRedirect");

const portalUrl = "https://billing.stripe.test/session";

describe("When a payment failed", () => {
  beforeEach(() => {
    Platform.OS = "web";
    jest.mocked(getOrganizationSubscription).mockResolvedValue(
      buildOrganizationSubscription({
        planCode: "pro",
        isExternal: true,
        paymentStatus: "PastDue",
      }),
    );
    jest.mocked(openBillingPortal).mockResolvedValue({ url: portalUrl });
  });

  it("Then the owner can update the card", async () => {
    await renderWithProviders(<SubscriptionNotice />, {
      member: buildCurrentMember({ subscription: buildSubscription({ planCode: "pro" }) }),
    });

    expect(await screen.findByText(translate("subscriptions.pastDue"))).toBeTruthy();
    fireEvent.press(screen.getByText(translate("subscriptions.updateCard")));

    await waitFor(() => expect(goToBillingPage).toHaveBeenCalledWith(portalUrl));
  });
});
