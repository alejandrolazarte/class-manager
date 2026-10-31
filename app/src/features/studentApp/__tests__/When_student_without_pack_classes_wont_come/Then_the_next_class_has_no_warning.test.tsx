import { screen } from "@testing-library/react-native";
import { getStudentAppHome, getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { StudentAppHomeScreen } from "@/features/studentApp/screens/StudentAppHomeScreen";
import { translate } from "@/i18n/translate";
import {
  buildAccountStudent,
  buildStudentAppHome,
  buildStudentAppNextClass,
  buildStudentAppShop,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";
import { addDays, todayIsoDate } from "@/dates/dates";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student without pack classes won't come", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        students: [
          buildAccountStudent({
            nextClasses: [
              buildStudentAppNextClass({
                date: addDays(todayIsoDate(), 1),
                absenceNotified: true,
              }),
            ],
          }),
        ],
        billing: {
          monthlyFee: null,
          classes: { availableClasses: 0, unpaidClasses: 0 },
        },
      }),
    );
  });

  it("Then the next class has no warning", async () => {
    await renderStudentAppScreen(<StudentAppHomeScreen />);

    expect(await screen.findByText(translate("student.absence.notified"))).toBeOnTheScreen();
    expect(screen.queryByText(translate("student.nextClass.noClassesLeft"))).toBeNull();
  });
});
