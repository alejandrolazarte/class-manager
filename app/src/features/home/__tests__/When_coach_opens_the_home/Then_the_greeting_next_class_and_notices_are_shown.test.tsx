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
jest.mock("@/features/home/nextSession", () => ({
  ...jest.requireActual("@/features/home/nextSession"),
  currentTimeLabel: () => "10:00",
}));

const eveningClass = buildDaySession({
  classGroupName: "Natación nocturna",
  startTime: "18:00",
  endTime: "18:45",
});

describe("When coach opens the home", () => {
  beforeEach(() => {
    jest.mocked(listDaySessions).mockResolvedValue([eveningClass]);
    jest.mocked(getTeamNotifications).mockResolvedValue(buildTeamNotifications({ unreadCount: 2 }));
  });

  it("Then the greeting, next class and notices are shown", async () => {
    await renderWithProviders(<DayScreen />, {
      member: buildCurrentMember({ fullName: "Laura Gómez" }),
    });

    expect(
      await screen.findByText(translate("home.greeting", { name: "Laura" })),
    ).toBeOnTheScreen();
    expect(await screen.findByText("18:00–18:45")).toBeOnTheScreen();
    expect(
      screen.getByRole("button", { name: translate("home.nextSession.takeAttendance") }),
    ).toBeOnTheScreen();
    expect(await screen.findByText("2 nuevos")).toBeOnTheScreen();
  });
});
