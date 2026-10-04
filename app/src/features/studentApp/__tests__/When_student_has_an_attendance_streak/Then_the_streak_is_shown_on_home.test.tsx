import { screen } from "@testing-library/react-native";
import { getStudentAppHome, getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { StudentAppHomeScreen } from "@/features/studentApp/screens/StudentAppHomeScreen";
import { translate, translateCount } from "@/i18n/translate";
import {
  buildStudentAppAttendance,
  buildStudentAppHome,
  buildStudentAppShop,
  buildAccountStudent,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student has an attendance streak", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        students: [
          buildAccountStudent({
            attendance: buildStudentAppAttendance({ streakWeeks: 6, streakSince: "2026-08-10" }),
          }),
        ],
      }),
    );
  });

  it("Then the streak is shown on home", async () => {
    await renderStudentAppScreen(<StudentAppHomeScreen />);

    expect(await screen.findByText(translateCount("student.streak.weeks", 6))).toBeOnTheScreen();
    expect(
      screen.getByText(translate("student.streak.since", { month: "agosto" })),
    ).toBeOnTheScreen();
  });
});
