import { screen } from "@testing-library/react-native";
import { DayScreen } from "@/features/sessions/screens/DayScreen";
import { listDaySessions } from "@/features/sessions/sessionsApi";
import { getTeamNotifications } from "@/features/teamNotifications/teamNotificationsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildDaySession } from "@/testing/sessionFactory";
import { buildTeamNotifications } from "@/testing/teamNotificationFactory";

jest.mock("@/features/sessions/sessionsApi");
jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/teamNotifications/teamNotificationsApi");
jest.mock("@/features/home/nextSession", () => ({
  ...jest.requireActual("@/features/home/nextSession"),
  currentTimeLabel: () => "19:00",
}));

describe("When every class of the day ended", () => {
  beforeEach(() => {
    jest
      .mocked(listDaySessions)
      .mockResolvedValue([buildDaySession({ startTime: "18:00", endTime: "18:45" })]);
    jest.mocked(getTeamNotifications).mockResolvedValue(buildTeamNotifications());
  });

  it("Then no class is left for today", async () => {
    await renderWithProviders(<DayScreen />);

    expect(await screen.findByText(translate("home.nextSession.noneLeft"))).toBeOnTheScreen();
    expect(
      screen.queryByRole("button", { name: translate("home.nextSession.takeAttendance") }),
    ).toBeNull();
  });
});
