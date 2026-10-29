import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppClassPacks, getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppClassPacksScreen } from "@/features/studentApp/screens/StudentAppClassPacksScreen";
import { dayAndMonthLabel } from "@/features/studentApp/studentAppSchedule";
import { translateCount } from "@/i18n/translate";
import {
  buildStudentAppClassPacks,
  buildStudentAppHome,
  buildStudentAppPackClass,
  buildStudentAppPackPurchase,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student opens the classes of a pack", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(getStudentAppClassPacks).mockResolvedValue(
      buildStudentAppClassPacks({
        purchases: [
          buildStudentAppPackPurchase({
            status: "UsedUp",
            usedClasses: 2,
            remainingClasses: 0,
            classes: [
              buildStudentAppPackClass({ date: "2026-10-13" }),
              buildStudentAppPackClass({ date: "2026-10-11" }),
            ],
          }),
        ],
      }),
    );
  });

  it("Then the classes it paid are shown", async () => {
    await renderStudentAppScreen(<StudentAppClassPacksScreen />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translateCount("student.classPacks.showClasses", 2),
      }),
    );

    expect(screen.getByText(dayAndMonthLabel("2026-10-13"))).toBeOnTheScreen();
  });
});
