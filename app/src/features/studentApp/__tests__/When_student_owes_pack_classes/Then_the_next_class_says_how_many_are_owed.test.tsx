import { screen } from "@testing-library/react-native";
import { getStudentAppHome, getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { StudentAppHomeScreen } from "@/features/studentApp/screens/StudentAppHomeScreen";
import { translateCount } from "@/i18n/translate";
import {
  buildAccountStudent,
  buildStudentAppHome,
  buildStudentAppNextClass,
  buildStudentAppShop,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";
import { addDays, todayIsoDate } from "@/dates/dates";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student owes pack classes", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        students: [
          buildAccountStudent({
            nextClasses: [
              buildStudentAppNextClass({
                date: addDays(todayIsoDate(), 1),
                absenceNotified: false,
              }),
            ],
          }),
        ],
        billing: {
          kind: "ClassPacks",
          monthlyFee: null,
          classes: { availableClasses: 0, unpaidClasses: 2 },
        },
      }),
    );
  });

  it("Then the next class says how many are owed", async () => {
    await renderStudentAppScreen(<StudentAppHomeScreen />);

    expect(
      await screen.findByText(translateCount("student.nextClass.unpaidClasses", 2)),
    ).toBeOnTheScreen();
  });
});
