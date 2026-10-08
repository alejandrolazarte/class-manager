import { screen } from "@testing-library/react-native";
import { todayIsoDate } from "@/features/sessions/dates";
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
jest.mock("@/features/home/agenda", () => ({
  ...jest.requireActual("@/features/home/agenda"),
  currentTimeLabel: () => "10:00",
}));

describe("When instructor opens the home", () => {
  beforeEach(() => {
    jest.mocked(listDaySessions).mockResolvedValue([
      buildDaySession({
        date: todayIsoDate(),
        classGroupName: "Natación nocturna",
        startTime: "18:00",
        endTime: "18:45",
      }),
    ]);
    jest.mocked(getTeamNotifications).mockResolvedValue(buildTeamNotifications());
  });

  it("Then the greeting and the next class are shown", async () => {
    await renderWithProviders(<DayScreen />, {
      member: buildCurrentMember({ fullName: "Laura Gómez" }),
    });

    expect(
      await screen.findByText(translate("home.greeting", { name: "Laura" })),
    ).toBeOnTheScreen();
    expect(
      await screen.findByRole("button", {
        name: `${translate("home.banner.next")}: 18:00 · Natación nocturna`,
      }),
    ).toBeOnTheScreen();
    expect(screen.getByText(`${translate("home.banner.next")} · en 8 h`)).toBeOnTheScreen();
  });
});
