import { screen, waitFor } from "@testing-library/react-native";
import { TeamNotificationsScreen } from "@/features/teamNotifications/screens/TeamNotificationsScreen";
import {
  getTeamNotifications,
  markTeamNotificationsSeen,
} from "@/features/teamNotifications/teamNotificationsApi";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildTeamNotification, buildTeamNotifications } from "@/testing/teamNotificationFactory";

jest.mock("@/features/teamNotifications/teamNotificationsApi");

const notification = buildTeamNotification();

describe("When instructor opens the notifications", () => {
  beforeEach(() => {
    jest.mocked(markTeamNotificationsSeen).mockResolvedValue();
    jest
      .mocked(getTeamNotifications)
      .mockResolvedValue(buildTeamNotifications({ items: [notification], unreadCount: 1 }));
  });

  it("Then they are listed and marked seen", async () => {
    await renderWithProviders(<TeamNotificationsScreen />);

    expect(await screen.findByText(notification.title)).toBeOnTheScreen();
    expect(screen.getByText(notification.body)).toBeOnTheScreen();
    await waitFor(() => expect(markTeamNotificationsSeen).toHaveBeenCalled());
  });
});
