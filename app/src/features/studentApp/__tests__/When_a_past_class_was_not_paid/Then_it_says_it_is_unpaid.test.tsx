import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppClassHistory, getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppClassesScreen } from "@/features/studentApp/screens/StudentAppClassesScreen";
import { shortDayLabel } from "@/features/studentApp/studentAppSchedule";
import { addDays, monthOfDate, todayIsoDate } from "@/dates/dates";
import { translate } from "@/i18n/translate";
import {
  buildStudentAppClassHistory,
  buildStudentAppHome,
  buildStudentAppPastClass,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const lastWeek = addDays(todayIsoDate(), -7);

describe("When a past class was not paid", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(getStudentAppClassHistory).mockResolvedValue(
      buildStudentAppClassHistory({
        month: monthOfDate(lastWeek),
        classes: [buildStudentAppPastClass({ date: lastWeek, isUnpaid: true })],
      }),
    );
  });

  it("Then it says it is unpaid", async () => {
    await renderStudentAppScreen(<StudentAppClassesScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("sessions.calendar.previousWeek") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: shortDayLabel(lastWeek) }));

    expect(await screen.findByText(translate("student.history.unpaid"))).toBeOnTheScreen();
  });
});
