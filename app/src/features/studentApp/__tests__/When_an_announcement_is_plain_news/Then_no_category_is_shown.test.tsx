import { screen } from "@testing-library/react-native";
import { getStudentAppNews, markStudentAppNewsSeen } from "@/features/studentApp/studentAppApi";
import { StudentAppNewsScreen } from "@/features/studentApp/screens/StudentAppNewsScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppNews, buildStudentAppNewsItem } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When an announcement is plain news", () => {
  beforeEach(() => {
    jest.mocked(markStudentAppNewsSeen).mockResolvedValue();
    jest.mocked(getStudentAppNews).mockResolvedValue(
      buildStudentAppNews({
        items: [buildStudentAppNewsItem({ title: "Nuevo horario", category: "News" })],
      }),
    );
  });

  it("Then no category is shown", async () => {
    await renderStudentAppScreen(<StudentAppNewsScreen />);

    await screen.findByText("Nuevo horario");
    expect(screen.queryByText(translate("announcements.category.news"))).not.toBeOnTheScreen();
  });
});
