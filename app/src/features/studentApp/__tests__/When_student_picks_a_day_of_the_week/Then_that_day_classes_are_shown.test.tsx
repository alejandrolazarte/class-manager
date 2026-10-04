import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
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

const today = todayIsoDate();

describe("When student picks a day of the week", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        students: [
          buildAccountStudent({
            nextClasses: [
              buildStudentAppNextClass({
                name: "Natación inicial",
                date: today,
                startTime: "18:00",
              }),
              buildStudentAppNextClass({
                name: "Natación inicial",
                date: addDays(today, 7),
                startTime: "18:00",
                isCancelled: true,
              }),
            ],
          }),
        ],
      }),
    );
  });

  it("Then that day classes are shown", async () => {
    await renderStudentAppScreen(<StudentAppClassesScreen />);

    expect(await screen.findByText("18:00–18:45")).toBeOnTheScreen();

    await fireEvent.press(
      screen.getByRole("button", { name: translate("student.schedule.nextWeek") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: shortDayLabel(addDays(today, 7)) }));

    expect(screen.getByText(translate("student.student.cancelled"))).toBeOnTheScreen();
  });
});
