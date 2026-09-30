import { screen } from "@testing-library/react-native";
import { TeamNotificationBell } from "@/features/teamNotifications/components/TeamNotificationBell";
import { getTeamNotifications } from "@/features/teamNotifications/teamNotificationsApi";
import { translateCount } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildTeamNotifications } from "@/testing/teamNotificationFactory";

jest.mock("@/features/teamNotifications/teamNotificationsApi");

describe("When team has unread notifications", () => {
  beforeEach(() => {
    jest.mocked(getTeamNotifications).mockResolvedValue(buildTeamNotifications({ unreadCount: 3 }));
  });

  it("Then the bell shows how many", async () => {
    await renderWithProviders(<TeamNotificationBell />);

    expect(
      await screen.findByRole("button", { name: translateCount("family.news.bell", 3) }),
    ).toBeOnTheScreen();
  });
});
