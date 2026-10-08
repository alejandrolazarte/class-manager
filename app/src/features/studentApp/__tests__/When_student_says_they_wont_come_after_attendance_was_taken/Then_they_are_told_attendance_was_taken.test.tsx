import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import {
  getStudentAppHome,
  getStudentAppShop,
  notifyAbsence,
} from "@/features/studentApp/studentAppApi";
import { StudentAppHomeScreen } from "@/features/studentApp/screens/StudentAppHomeScreen";
import { addDays, todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import {
  buildStudentAppHome,
  buildStudentAppNextClass,
  buildStudentAppShop,
  buildAccountStudent,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const conflictStatus = 409;
const tomorrow = addDays(todayIsoDate(), 1);

describe("When student says they won't come after attendance was taken", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
    jest
      .mocked(notifyAbsence)
      .mockRejectedValue(new ApiError(conflictStatus, { code: "absence.attendance_taken" }));
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        students: [
          buildAccountStudent({
            nextClasses: [buildStudentAppNextClass({ date: tomorrow })],
          }),
        ],
      }),
    );
  });

  it("Then they are told attendance was taken", async () => {
    await renderStudentAppScreen(<StudentAppHomeScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.absence.notGoing") }),
    );

    expect(await screen.findByText(translate("student.absence.attendanceTaken"))).toBeOnTheScreen();
  });
});
