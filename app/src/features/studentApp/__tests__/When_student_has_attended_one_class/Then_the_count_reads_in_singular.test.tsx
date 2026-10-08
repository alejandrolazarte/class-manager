import { screen } from "@testing-library/react-native";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppProgressScreen } from "@/features/studentApp/screens/StudentAppProgressScreen";
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
        students: [
          buildAccountStudent({
            attendance: buildStudentAppAttendance({ attendedClasses: 1, streakWeeks: 3 }),
          }),
        ],
      }),
    );
  });

  it("Then the count reads in singular", async () => {
    await renderStudentAppScreen(<StudentAppProgressScreen />);

    expect(
      await screen.findByText(translateCount("student.progress.stats.classes", 1)),
    ).toBeOnTheScreen();
  });
});
