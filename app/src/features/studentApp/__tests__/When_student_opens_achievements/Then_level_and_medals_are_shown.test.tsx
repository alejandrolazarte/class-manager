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

describe("When student opens achievements", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        levels: [
          { name: "Inicial", requiredClasses: 0 },
          { name: "Base", requiredClasses: 10 },
          { name: "Explorador", requiredClasses: 25 },
        ],
        students: [
          buildAccountStudent({
            attendance: buildStudentAppAttendance({
              attendedClasses: 18,
              level: 2,
              medals: ["FirstClass", "TenClasses", "LeveledUp"],
            }),
          }),
        ],
      }),
    );
  });

  it("Then level and medals are shown", async () => {
    await renderStudentAppScreen(<StudentAppProgressScreen />);

    expect(
      await screen.findByText(translate("student.progress.level", { number: 2, name: "Base" })),
    ).toBeOnTheScreen();
    expect(
      screen.getByText(translateCount("student.progress.classesToNext", 7, { name: "Explorador" })),
    ).toBeOnTheScreen();
    expect(
      screen.getByLabelText(
        translate("student.progress.medalEarned", { name: translate("student.medals.TenClasses") }),
      ),
    ).toBeOnTheScreen();
    expect(
      screen.getByLabelText(
        translate("student.progress.medalLocked", {
          name: translate("student.medals.HundredClasses"),
        }),
      ),
    ).toBeOnTheScreen();
  });
});
