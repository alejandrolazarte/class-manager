import { screen, within } from "@testing-library/react-native";
import { getStudentAppNews, markStudentAppNewsSeen } from "@/features/studentApp/studentAppApi";
import { StudentAppNewsScreen } from "@/features/studentApp/screens/StudentAppNewsScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppNews, buildStudentAppNewsItem } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When a pinned announcement has a category", () => {
  beforeEach(() => {
    jest.mocked(markStudentAppNewsSeen).mockResolvedValue();
    jest.mocked(getStudentAppNews).mockResolvedValue(
      buildStudentAppNews({
        items: [
          buildStudentAppNewsItem({
            announcementId: "announcement-1",
            title: "Muestra de fin de año",
            category: "Event",
            isPinned: true,
          }),
        ],
      }),
    );
  });

  it("Then it shows the category", async () => {
    await renderStudentAppScreen(<StudentAppNewsScreen />);

    const pinned = within(await screen.findByTestId("pinned-news"));
    expect(pinned.getByText(translate("announcements.category.event"))).toBeOnTheScreen();
  });
});
