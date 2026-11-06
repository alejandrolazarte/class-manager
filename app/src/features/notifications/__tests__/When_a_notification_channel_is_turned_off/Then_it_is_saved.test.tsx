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

describe("When a notification channel is turned off", () => {
  beforeEach(() => {
    const orders = buildNotificationPreference("Orders", "Family");
    jest
      .mocked(getMyNotificationPreferences)
      .mockResolvedValue(buildNotificationPreferences([orders]));
    jest.mocked(updateMyNotificationPreference).mockResolvedValue({
      ...orders,
      channels: [
        { channel: "Email", isEnabled: false },
        { channel: "Push", isEnabled: true },
      ],
    });
  });

  it("Then it is saved", async () => {
    await renderWithProviders(
      <NotificationPreferencesScreen notifications={pushUnavailable} deviceHint="" />,
    );

    await fireEvent.press(await screen.findByRole("switch", { name: ordersEmailLabel }));

    await waitFor(() =>
      expect(updateMyNotificationPreference).toHaveBeenCalledWith({
        category: "Orders",
        channel: "Email",
        isEnabled: false,
      }),
    );
    expect(screen.getByRole("switch", { name: ordersEmailLabel })).not.toBeChecked();
  });
});
