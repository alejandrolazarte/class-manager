import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getStudentAppHome, withdrawAbsence } from "@/features/studentApp/studentAppApi";
import { StudentAppClassesScreen } from "@/features/studentApp/screens/StudentAppClassesScreen";
import { shortDayLabel } from "@/features/studentApp/studentAppSchedule";
import { addDays, todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import {
  buildStudentAppHome,
  buildStudentAppNextClass,
  buildAccountStudent,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const nextWeek = addDays(todayIsoDate(), 7);

describe("When student takes back a notice", () => {
  beforeEach(() => {
    jest.mocked(withdrawAbsence).mockResolvedValue();
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        students: [
          buildAccountStudent({
            id: "student-tomas",
            nextClasses: [
              buildStudentAppNextClass({
                date: nextWeek,
                classGroupId: "class-group-1",
                absenceNotified: true,
              }),
            ],
          }),
        ],
      }),
    );
  });

  it("Then the withdrawal is sent", async () => {
    await renderStudentAppScreen(<StudentAppClassesScreen />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.schedule.nextWeek") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: shortDayLabel(nextWeek) }));

    expect(screen.getByText(translate("student.absence.status"))).toBeOnTheScreen();
    await fireEvent.press(screen.getByRole("button", { name: translate("student.absence.going") }));

    await waitFor(() =>
      expect(withdrawAbsence).toHaveBeenCalledWith("student-tomas", "class-group-1", nextWeek),
    );
  });
});
