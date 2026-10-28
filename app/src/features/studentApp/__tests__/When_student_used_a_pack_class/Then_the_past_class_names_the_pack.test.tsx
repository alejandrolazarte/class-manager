import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppClassHistory, getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppClassesScreen } from "@/features/studentApp/screens/StudentAppClassesScreen";
import { dayAndMonthLabel, shortDayLabel } from "@/features/studentApp/studentAppSchedule";
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
const purchasedOn = addDays(todayIsoDate(), -20);

describe("When student used a pack class", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(getStudentAppClassHistory).mockResolvedValue(
      buildStudentAppClassHistory({
        month: monthOfDate(lastWeek),
        classes: [
          buildStudentAppPastClass({
            date: lastWeek,
            paidWithPack: { name: "Pack 8 clases", purchasedOn },
          }),
        ],
      }),
    );
  });

  it("Then the past class names the pack", async () => {
    await renderStudentAppScreen(<StudentAppClassesScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("sessions.calendar.previousWeek") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: shortDayLabel(lastWeek) }));

    expect(
      await screen.findByText(
        translate("student.history.paidWithPack", {
          pack: "Pack 8 clases",
          date: dayAndMonthLabel(purchasedOn),
        }),
      ),
    ).toBeOnTheScreen();
  });
});
