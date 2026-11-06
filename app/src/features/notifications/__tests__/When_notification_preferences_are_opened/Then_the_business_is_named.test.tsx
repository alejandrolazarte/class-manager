import { screen } from "@testing-library/react-native";
import { getMyNotificationPreferences } from "@/features/notifications/notificationPreferencesApi";
import { NotificationPreferencesScreen } from "@/features/notifications/screens/NotificationPreferencesScreen";
import { translate } from "@/i18n/translate";
import {
  buildNotificationPreference,
  buildNotificationPreferences,
  pushUnavailable,
  notificationPreferencesBusinessName,
} from "@/testing/notificationPreferenceFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/notifications/notificationPreferencesApi");

describe("When notification preferences are opened", () => {
  beforeEach(() => {
    jest
      .mocked(getMyNotificationPreferences)
      .mockResolvedValue(
        buildNotificationPreferences([buildNotificationPreference("Orders", "Family")]),
      );
  });

  it("Then the business is named", async () => {
    await renderWithProviders(
      <NotificationPreferencesScreen notifications={pushUnavailable} deviceHint="" />,
    );

    expect(
      await screen.findByText(
        translate("notificationPreferences.business", {
          business: notificationPreferencesBusinessName,
        }),
      ),
    ).toBeOnTheScreen();
  });
});
