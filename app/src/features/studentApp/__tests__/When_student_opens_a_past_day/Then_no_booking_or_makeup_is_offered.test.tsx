import { fireEvent, screen } from "@testing-library/react-native";
import {
  getStudentAppClassHistory,
  getStudentAppHome,
  getStudentAppMakeups,
  getStudentAppPackClasses,
} from "@/features/studentApp/studentAppApi";
import { StudentAppClassesScreen } from "@/features/studentApp/screens/StudentAppClassesScreen";
import { shortDayLabel } from "@/features/studentApp/studentAppSchedule";
import { addDays, monthOfDate, todayIsoDate } from "@/dates/dates";
import { translate } from "@/i18n/translate";
import {
  buildStudentAppClassHistory,
  buildStudentAppHome,
  buildStudentAppMakeups,
  buildStudentAppMakeupSlot,
  buildStudentAppPastClass,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const today = todayIsoDate();
const lastWeek = addDays(today, -7);

describe("When student opens a past day", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(getStudentAppMakeups).mockResolvedValue(buildStudentAppMakeups());
    jest.mocked(getStudentAppPackClasses).mockResolvedValue({
      classesLeft: 4,
      slots: [buildStudentAppMakeupSlot({ date: today })],
    });
    jest.mocked(getStudentAppClassHistory).mockResolvedValue(
      buildStudentAppClassHistory({
        month: monthOfDate(lastWeek),
        classes: [buildStudentAppPastClass({ date: lastWeek })],
      }),
    );
  });

  it("Then no booking or makeup is offered", async () => {
    await renderStudentAppScreen(<StudentAppClassesScreen />);
    expect(await screen.findByText(translate("student.makeup.title"))).toBeOnTheScreen();

    await fireEvent.press(
      screen.getByRole("button", { name: translate("sessions.calendar.previousWeek") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: shortDayLabel(lastWeek) }));

    expect(await screen.findByText(translate("student.history.attended"))).toBeOnTheScreen();
    expect(screen.queryByText(translate("student.makeup.title"))).toBeNull();
    expect(screen.queryByText(translate("student.packClasses.title"))).toBeNull();
  });
});
