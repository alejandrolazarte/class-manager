import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getStudentAppHome, withdrawAbsence } from "@/features/studentApp/studentAppApi";
import { StudentAppClassScreen } from "@/features/studentApp/screens/StudentAppClassScreen";
import { addDays, todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { searchParametersMock } from "@/testing/expoRouterMock";
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
    searchParametersMock.current = {
      classGroupId: "class-group-1",
      date: nextWeek,
      startTime: "18:00",
    };
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        students: [
          buildAccountStudent({
            id: "student-tomas",
            nextClasses: [
              buildStudentAppNextClass({
                date: nextWeek,
                startTime: "18:00",
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
    await renderStudentAppScreen(<StudentAppClassScreen />);

    expect(await screen.findByText(translate("student.absence.status"))).toBeOnTheScreen();
    await fireEvent.press(screen.getByRole("button", { name: translate("student.absence.going") }));

    await waitFor(() =>
      expect(withdrawAbsence).toHaveBeenCalledWith("student-tomas", "class-group-1", nextWeek),
    );
  });
});
