import { screen } from "@testing-library/react-native";
import { todayIsoDate } from "@/dates/dates";
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
jest.mock("@/features/home/agenda", () => ({
  ...jest.requireActual("@/features/home/agenda"),
  currentTimeLabel: () => "19:00",
}));

describe("When every class of the day ended", () => {
  beforeEach(() => {
    jest.mocked(listDaySessions).mockResolvedValue([
      buildDaySession({
        date: todayIsoDate(),
        startTime: "18:00",
        endTime: "18:45",
        presentCount: 4,
      }),
    ]);
    jest.mocked(getTeamNotifications).mockResolvedValue(buildTeamNotifications());
  });

  it("Then no class is left for today", async () => {
    await renderWithProviders(<DayScreen />);

    expect(await screen.findByText(translate("home.banner.noneLeft"))).toBeOnTheScreen();
    expect(screen.getByText(translate("home.status.taken"))).toBeOnTheScreen();
    expect(screen.queryByRole("button", { name: translate("home.banner.attendance") })).toBeNull();
  });
});
