import { screen } from "@testing-library/react-native";
import { listMyNotificationPreferences } from "@/features/notifications/notificationPreferencesApi";
import { NotificationPreferencesScreen } from "@/features/notifications/screens/NotificationPreferencesScreen";
import { translate } from "@/i18n/translate";
import { buildNotificationPreference } from "@/testing/notificationPreferenceFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/notifications/notificationPreferencesApi");

describe("When notification preferences are opened", () => {
  beforeEach(() => {
    jest
      .mocked(listMyNotificationPreferences)
      .mockResolvedValue([buildNotificationPreference("Orders", "Family")]);
  });

  it("Then only the groups that apply are shown", async () => {
    await renderWithProviders(<NotificationPreferencesScreen />);

    expect(
      await screen.findByText(translate("notificationPreferences.audience.Family")),
    ).toBeOnTheScreen();
    expect(screen.queryByText(translate("notificationPreferences.audience.Team"))).toBeNull();
  });
});
