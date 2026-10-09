import { screen } from "@testing-library/react-native";
import { getStudentAppNews, markStudentAppNewsSeen } from "@/features/studentApp/studentAppApi";
import { StudentAppNewsScreen } from "@/features/studentApp/screens/StudentAppNewsScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppNews, buildStudentAppNewsItem } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When a class changes in the news", () => {
  beforeEach(() => {
    jest.mocked(markStudentAppNewsSeen).mockResolvedValue();
    jest.mocked(getStudentAppNews).mockResolvedValue(
      buildStudentAppNews({
        items: [
          buildStudentAppNewsItem({
            id: "changed",
            kind: "ClassChanged",
            title: null,
            body: null,
            className: "Grupo avanzado",
            classDate: "2026-10-15",
            classStartTime: "20:30",
            instructorFullName: "Paula Ruiz",
          }),
        ],
      }),
    );
  });

  it("Then it tells the new time and instructor", async () => {
    await renderStudentAppScreen(<StudentAppNewsScreen />);

    expect(
      await screen.findByText(
        `Grupo avanzado · ${translate("student.news.changed.time", { time: "20:30" })} · ${translate("student.news.changed.instructor", { instructor: "Paula" })}`,
      ),
    ).toBeOnTheScreen();
  });
});
