import { screen, waitFor } from "@testing-library/react-native";
import { getStudentAppNews, markStudentAppNewsSeen } from "@/features/studentApp/studentAppApi";
import { StudentAppNewsScreen } from "@/features/studentApp/screens/StudentAppNewsScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppNews, buildStudentAppNewsItem } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student opens the news", () => {
  beforeEach(() => {
    jest.mocked(markStudentAppNewsSeen).mockResolvedValue();
    jest.mocked(getStudentAppNews).mockResolvedValue(
      buildStudentAppNews({
        items: [
          buildStudentAppNewsItem({
            id: "today",
            title: "Lunes 12 cerrado",
            occurredAt: new Date().toISOString(),
          }),
          buildStudentAppNewsItem({
            id: "old",
            title: "Muestra de fin de año",
            occurredAt: "2026-01-10T12:00:00Z",
            isUnread: false,
          }),
        ],
        unreadCount: 1,
      }),
    );
  });

  it("Then they are grouped and marked seen", async () => {
    await renderStudentAppScreen(<StudentAppNewsScreen />);

    expect(await screen.findByText("Lunes 12 cerrado")).toBeOnTheScreen();
    expect(screen.getByText(translate("student.news.group.today"))).toBeOnTheScreen();
    expect(screen.getByText(translate("student.news.group.earlier"))).toBeOnTheScreen();
    await waitFor(() => expect(markStudentAppNewsSeen).toHaveBeenCalled());
  });
});
