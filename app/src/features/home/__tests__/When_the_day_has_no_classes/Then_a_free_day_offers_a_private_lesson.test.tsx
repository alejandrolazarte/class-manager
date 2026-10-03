import { fireEvent, screen } from "@testing-library/react-native";
import { DayScreen } from "@/features/sessions/screens/DayScreen";
import { listDaySessions } from "@/features/sessions/sessionsApi";
import { getTeamNotifications } from "@/features/teamNotifications/teamNotificationsApi";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { sessionDate } from "@/testing/sessionFactory";
import { buildTeamNotifications } from "@/testing/teamNotificationFactory";

jest.mock("@/features/sessions/sessionsApi");
jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/teamNotifications/teamNotificationsApi");

describe("When the day has no classes", () => {
  beforeEach(() => {
    jest.mocked(listDaySessions).mockResolvedValue([]);
    jest.mocked(getTeamNotifications).mockResolvedValue(buildTeamNotifications());
  });

  it("Then a free day offers a private lesson", async () => {
    await renderWithProviders(<DayScreen initialDate={sessionDate} />);

    expect(await screen.findByText(translate("home.free.title"))).toBeOnTheScreen();
    const privateLessonButtons = screen.getAllByRole("button", {
      name: translate("privateLessons.new"),
    });
    expect(privateLessonButtons).toHaveLength(1);
    await fireEvent.press(privateLessonButtons[0]);

    expect(routerMock.push).toHaveBeenCalledWith(routes.newPrivateLesson(sessionDate));
  });
});
