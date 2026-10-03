import { fireEvent, screen } from "@testing-library/react-native";
import { Text } from "react-native";
import { SubscriptionGate } from "@/features/subscriptions/components/SubscriptionGate";
import { listPlans } from "@/features/subscriptions/subscriptionsApi";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildPlans, buildSubscription } from "@/testing/subscriptionFactory";

jest.mock("@/features/subscriptions/subscriptionsApi");

const appContent = "Pestañas del equipo";

describe("When the owner chooses to see their data", () => {
  beforeEach(() => {
    jest.mocked(listPlans).mockResolvedValue(buildPlans());
  });

  it("Then the app opens read only", async () => {
    const subscription = buildSubscription({
      isActive: false,
      expiredOn: "2026-09-30",
      features: [],
    });
    await renderWithProviders(
      <SubscriptionGate>
        <Text>{appContent}</Text>
      </SubscriptionGate>,
      { member: buildCurrentMember({ subscription }) },
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("subscriptions.ended.seeData") }),
    );

    expect(screen.getByText(appContent)).toBeTruthy();
  });
});
