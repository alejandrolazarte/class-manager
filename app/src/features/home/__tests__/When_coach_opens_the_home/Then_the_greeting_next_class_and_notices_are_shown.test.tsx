import { screen } from "@testing-library/react-native";
import { DayScreen } from "@/features/sessions/screens/DayScreen";
import { listDaySessions } from "@/features/sessions/sessionsApi";
import { getTeamNotifications } from "@/features/teamNotifications/teamNotificationsApi";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildDaySession } from "@/testing/sessionFactory";
import { buildTeamNotifications } from "@/testing/teamNotificationFactory";

jest.mock("@/features/sessions/sessionsApi");
jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/teamNotifications/teamNotificationsApi");

const lateClass = buildDaySession({
  classGroupName: "Natación nocturna",
  startTime: "23:58",
  endTime: "23:59",
});

describe("When coach opens the home", () => {
  beforeEach(() => {
    jest.mocked(listDaySessions).mockResolvedValue([lateClass]);
    jest.mocked(getTeamNotifications).mockResolvedValue(buildTeamNotifications({ unreadCount: 2 }));
  });

  it("Then the greeting, next class and notices are shown", async () => {
    await renderWithProviders(<DayScreen />, {
      member: buildCurrentMember({ fullName: "Laura Gómez" }),
    });

    expect(
      await screen.findByText(translate("home.greeting", { name: "Laura" })),
    ).toBeOnTheScreen();
    expect(await screen.findByText("23:58–23:59")).toBeOnTheScreen();
    expect(
      screen.getByRole("button", { name: translate("home.nextSession.takeAttendance") }),
    ).toBeOnTheScreen();
    expect(await screen.findByText("2 nuevos")).toBeOnTheScreen();
  });
});
