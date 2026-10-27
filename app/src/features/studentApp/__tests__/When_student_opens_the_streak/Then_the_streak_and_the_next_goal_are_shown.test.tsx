import { screen } from "@testing-library/react-native";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppStreakScreen } from "@/features/studentApp/screens/StudentAppStreakScreen";
import { translateCount } from "@/i18n/translate";
import {
  buildStudentAppAttendance,
  buildStudentAppHome,
  buildAccountStudent,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student opens the streak", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        students: [
          buildAccountStudent({
            attendance: buildStudentAppAttendance({
              streakWeeks: 6,
              bestStreakWeeks: 9,
              recentWeeks: [
                { weekStart: "2026-10-12", attendance: "Missed" },
                { weekStart: "2026-10-19", attendance: "Attended" },
              ],
            }),
          }),
        ],
      }),
    );
  });

  it("Then the streak and the next goal are shown", async () => {
    await renderStudentAppScreen(<StudentAppStreakScreen />);

    expect(
      await screen.findByText(translateCount("student.progress.streakWeeks", 6)),
    ).toBeOnTheScreen();
    expect(screen.getByText(translateCount("student.progress.bestStreak", 9))).toBeOnTheScreen();
    expect(screen.getByText(translateCount("student.streak.nextGoal", 8))).toBeOnTheScreen();
  });
});
