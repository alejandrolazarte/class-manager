import { fireEvent, screen } from "@testing-library/react-native";
import { todayIsoDate } from "@/features/sessions/dates";
import { DayScreen } from "@/features/sessions/screens/DayScreen";
import { listDaySessions } from "@/features/sessions/sessionsApi";
import { getTeamNotifications } from "@/features/teamNotifications/teamNotificationsApi";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildDaySession } from "@/testing/sessionFactory";
import { buildTeamNotifications } from "@/testing/teamNotificationFactory";

jest.mock("@/features/sessions/sessionsApi");
jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/teamNotifications/teamNotificationsApi");
jest.mock("@/features/home/agenda", () => ({
  ...jest.requireActual("@/features/home/agenda"),
  currentTimeLabel: () => "18:10",
}));

const runningClass = buildDaySession({
  date: todayIsoDate(),
  startTime: "18:00",
  endTime: "18:45",
});

describe("When a class is running", () => {
  beforeEach(() => {
    jest.mocked(listDaySessions).mockResolvedValue([runningClass]);
    jest.mocked(getTeamNotifications).mockResolvedValue(buildTeamNotifications());
  });

  it("Then attendance opens from the banner", async () => {
    await renderWithProviders(<DayScreen />);

    expect(
      await screen.findByText(translate("home.banner.live", { end: "18:45" })),
    ).toBeOnTheScreen();
    await fireEvent.press(
      screen.getByRole("button", { name: translate("home.banner.attendance") }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(
      routes.session(runningClass.classGroupId ?? "", runningClass.date),
    );
  });
});
