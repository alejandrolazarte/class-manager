import { screen } from "@testing-library/react-native";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppLevelsScreen } from "@/features/studentApp/screens/StudentAppLevelsScreen";
import { translateCount } from "@/i18n/translate";
import {
  buildStudentAppAttendance,
  buildStudentAppHome,
  buildAccountStudent,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student has attended one class", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        levels: [
          { name: "Inicial", requiredClasses: 0 },
          { name: "Base", requiredClasses: 10 },
        ],
        students: [
          buildAccountStudent({
            attendance: buildStudentAppAttendance({ attendedClasses: 1, streakWeeks: 3 }),
          }),
        ],
      }),
    );
  });

  it("Then the count reads in singular", async () => {
    await renderStudentAppScreen(<StudentAppLevelsScreen />);

    expect(
      await screen.findByText(translateCount("student.levels.summary", 1, { level: 1, total: 2 })),
    ).toBeOnTheScreen();
  });
});
