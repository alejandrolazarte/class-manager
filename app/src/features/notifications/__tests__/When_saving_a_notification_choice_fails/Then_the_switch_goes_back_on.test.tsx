import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  getMyNotificationPreferences,
  updateMyNotificationPreference,
} from "@/features/notifications/notificationPreferencesApi";
import { NotificationPreferencesScreen } from "@/features/notifications/screens/NotificationPreferencesScreen";
import { translate } from "@/i18n/translate";
import {
  buildNotificationPreference,
  buildNotificationPreferences,
  pushUnavailable,
} from "@/testing/notificationPreferenceFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/notifications/notificationPreferencesApi");

const ordersEmailLabel = translate("notificationPreferences.channelAccessibility", {
  category: translate("notificationPreferences.category.Orders"),
  channel: translate("notificationPreferences.channel.Email"),
});

describe("When saving a notification choice fails", () => {
  beforeEach(() => {
    jest
      .mocked(getMyNotificationPreferences)
      .mockResolvedValue(
        buildNotificationPreferences([buildNotificationPreference("Orders", "Family")]),
      );
    jest.mocked(updateMyNotificationPreference).mockRejectedValue(new Error("offline"));
  });

  it("Then the switch goes back on", async () => {
    await renderWithProviders(
      <NotificationPreferencesScreen notifications={pushUnavailable} deviceHint="" />,
    );

    await fireEvent.press(await screen.findByRole("switch", { name: ordersEmailLabel }));

    expect(await screen.findByText(translate("common.unexpectedError"))).toBeOnTheScreen();
    await waitFor(() =>
      expect(screen.getByRole("switch", { name: ordersEmailLabel })).toBeChecked(),
    );
  });
});
