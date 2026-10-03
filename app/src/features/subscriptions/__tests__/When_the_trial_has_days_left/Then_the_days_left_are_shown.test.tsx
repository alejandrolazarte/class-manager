import { screen } from "@testing-library/react-native";
import { SubscriptionNotice } from "@/features/subscriptions/components/SubscriptionNotice";
import { addDays, todayIsoDate } from "@/features/sessions/dates";
import { translateCount } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSubscription } from "@/testing/subscriptionFactory";

const daysLeft = 5;

describe("When the trial has days left", () => {
  it("Then the days left are shown", async () => {
    const subscription = buildSubscription({ endsOn: addDays(todayIsoDate(), daysLeft - 1) });

    await renderWithProviders(<SubscriptionNotice />, {
      member: buildCurrentMember({ subscription }),
    });

    expect(screen.getByText(translateCount("subscriptions.trialDaysLeft", daysLeft))).toBeTruthy();
  });
});
