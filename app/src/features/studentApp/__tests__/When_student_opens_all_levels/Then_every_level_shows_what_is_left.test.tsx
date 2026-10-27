import { screen } from "@testing-library/react-native";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppLevelsScreen } from "@/features/studentApp/screens/StudentAppLevelsScreen";
import { translate, translateCount } from "@/i18n/translate";
import {
  buildStudentAppAttendance,
  buildStudentAppHome,
  buildAccountStudent,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student opens all levels", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        levels: [
          { name: "Inicial", requiredClasses: 0 },
          { name: "Base", requiredClasses: 10 },
          { name: "Explorador", requiredClasses: 25 },
          { name: "Leyenda", requiredClasses: 60 },
        ],
        students: [
          buildAccountStudent({
            attendance: buildStudentAppAttendance({ attendedClasses: 18, level: 2 }),
          }),
        ],
      }),
    );
  });

  it("Then every level shows what is left", async () => {
    await renderStudentAppScreen(<StudentAppLevelsScreen />);

    expect(await screen.findByText("Leyenda")).toBeOnTheScreen();
    expect(screen.getByText(translate("student.levels.achieved"))).toBeOnTheScreen();
    expect(screen.getByText(translate("student.progress.currentLevel"))).toBeOnTheScreen();
    expect(screen.getByText(translateCount("student.levels.classesLeft", 7))).toBeOnTheScreen();
    expect(screen.getByText(translateCount("student.levels.classesLeft", 42))).toBeOnTheScreen();
    expect(screen.getByText(translate("student.levels.finalGoal"))).toBeOnTheScreen();
  });
});
