import { fireEvent, screen, waitFor } from "@testing-library/react-native";
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

const tomorrow = addDays(todayIsoDate(), 1);

describe("When student says the student won't come", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
    jest.mocked(notifyAbsence).mockResolvedValue();
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        students: [
          buildAccountStudent({
            id: "student-tomas",
            nextClasses: [
              buildStudentAppNextClass({ date: tomorrow, classGroupId: "class-group-1" }),
            ],
          }),
        ],
      }),
    );
  });

  it("Then the notice is sent", async () => {
    await renderStudentAppScreen(<StudentAppHomeScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.absence.notGoing") }),
    );

    await waitFor(() =>
      expect(notifyAbsence).toHaveBeenCalledWith("student-tomas", "class-group-1", tomorrow),
    );
  });
});
