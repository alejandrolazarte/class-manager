import { screen } from "@testing-library/react-native";
import { getStudentAppNews, markStudentAppNewsSeen } from "@/features/studentApp/studentAppApi";
import { StudentAppNewsScreen } from "@/features/studentApp/screens/StudentAppNewsScreen";
import { translate, translateCount } from "@/i18n/translate";
import { buildStudentAppNews, buildStudentAppNewsItem } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When an announcement is for the student class", () => {
  beforeEach(() => {
    jest.mocked(markStudentAppNewsSeen).mockResolvedValue();
    jest.mocked(getStudentAppNews).mockResolvedValue(
      buildStudentAppNews({
        items: [
          buildStudentAppNewsItem({
            title: "Cambia el horario",
            category: "Important",
            audienceClassNames: ["Nivel inicial"],
          }),
        ],
      }),
    );
  });

  it("Then its category and class are shown", async () => {
    await renderStudentAppScreen(<StudentAppNewsScreen />);

    expect(
      await screen.findByText(translate("announcements.category.important")),
    ).toBeOnTheScreen();
    expect(
      screen.getByText(translateCount("student.news.audience", 1, { classes: "Nivel inicial" })),
    ).toBeOnTheScreen();
  });
});
