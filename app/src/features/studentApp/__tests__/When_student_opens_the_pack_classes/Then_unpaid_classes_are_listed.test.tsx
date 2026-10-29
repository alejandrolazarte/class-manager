import { screen } from "@testing-library/react-native";
import { getStudentAppClassPacks, getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppClassPacksScreen } from "@/features/studentApp/screens/StudentAppClassPacksScreen";
import { dayAndMonthLabel } from "@/features/studentApp/studentAppSchedule";
import {
  buildStudentAppClassPacks,
  buildStudentAppHome,
  buildStudentAppPackClass,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student opens the pack classes", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(getStudentAppClassPacks).mockResolvedValue(
      buildStudentAppClassPacks({
        unpaidClasses: [buildStudentAppPackClass({ date: "2026-10-27" })],
      }),
    );
  });

  it("Then unpaid classes are listed", async () => {
    await renderStudentAppScreen(<StudentAppClassPacksScreen />);

    expect(await screen.findByText(dayAndMonthLabel("2026-10-27"))).toBeOnTheScreen();
  });
});
