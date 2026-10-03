import { screen } from "@testing-library/react-native";
import { Text } from "react-native";
import { SubscriptionGate } from "@/features/subscriptions/components/SubscriptionGate";
import { listPlans } from "@/features/subscriptions/subscriptionsApi";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildPlans, buildSubscription } from "@/testing/subscriptionFactory";

jest.mock("@/features/subscriptions/subscriptionsApi");

const appContent = "Pestañas del equipo";

describe("When the subscription has ended", () => {
  beforeEach(() => {
    jest.mocked(listPlans).mockResolvedValue(buildPlans());
  });

  it("Then the app asks to choose a plan", async () => {
    const subscription = buildSubscription({ isActive: false, endsOn: "2026-09-30", features: [] });

    await renderWithProviders(
      <SubscriptionGate>
        <Text>{appContent}</Text>
      </SubscriptionGate>,
      { member: buildCurrentMember({ subscription }) },
    );

    expect(screen.getByText(translate("subscriptions.ended.trialTitle"))).toBeTruthy();
    expect(await screen.findByText(translate("subscriptions.plan.lite"))).toBeTruthy();
    expect(screen.queryByText(appContent)).toBeNull();
  });
});
