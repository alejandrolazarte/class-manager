import { screen } from "@testing-library/react-native";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppProgressScreen } from "@/features/studentApp/screens/StudentAppProgressScreen";
import { translate, translateCount } from "@/i18n/translate";
import {
  buildStudentAppAttendance,
  buildStudentAppHome,
  buildAccountStudent,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");
jest.mock("@/features/studentApp/seenMedalsStorage");

describe("When student is close to a medal", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        students: [
          buildAccountStudent({
            attendance: buildStudentAppAttendance({
              attendedClasses: 7,
              streakWeeks: 1,
              medals: ["FirstClass"],
            }),
          }),
        ],
      }),
    );
  });

  it("Then the next medal is shown", async () => {
    await renderStudentAppScreen(<StudentAppProgressScreen />);

    expect(
      await screen.findByText(
        translateCount("student.progress.nextMedal.classes", 3, {
          name: translate("student.medals.TenClasses"),
        }),
      ),
    ).toBeOnTheScreen();
    expect(screen.getByText(translate("student.progress.nextMedal.title"))).toBeOnTheScreen();
  });
});
