import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppClassHistory, getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppClassesScreen } from "@/features/studentApp/screens/StudentAppClassesScreen";
import { addMonthsToDate, monthOfDate, todayIsoDate } from "@/dates/dates";
import { translate, translateCount } from "@/i18n/translate";
import {
  buildStudentAppClassHistory,
  buildStudentAppHome,
  buildStudentAppPastClass,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const lastMonthDay = addMonthsToDate(todayIsoDate(), -1);

describe("When student looks at a past month", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(getStudentAppClassHistory).mockImplementation(async (_studentId, month) =>
      buildStudentAppClassHistory({
        month,
        classes:
          month === monthOfDate(lastMonthDay)
            ? [
                buildStudentAppPastClass({ date: lastMonthDay }),
                buildStudentAppPastClass({
                  date: lastMonthDay,
                  startTime: "19:00",
                  status: "Absent",
                }),
              ]
            : [],
      }),
    );
  });

  it("Then the month shows how many classes they came to", async () => {
    await renderStudentAppScreen(<StudentAppClassesScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("home.view.month") }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("sessions.calendar.previousMonth") }),
    );

    expect(
      await screen.findByText(translateCount("student.history.monthSummary", 2, { attended: 1 })),
    ).toBeOnTheScreen();
  });
});
