import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { cancelPackClass } from "@/features/studentApp/studentAppApi";
import { ScheduledClassRow } from "@/features/studentApp/components/ScheduledClassRow";
import { addDays, todayIsoDate } from "@/dates/dates";
import { translate } from "@/i18n/translate";
import { buildStudentAppNextClass } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const tomorrow = addDays(todayIsoDate(), 1);

describe("When student cancels a pack class", () => {
  beforeEach(() => {
    jest.mocked(cancelPackClass).mockResolvedValue();
  });

  it("Then the booking is withdrawn", async () => {
    await renderStudentAppScreen(
      <ScheduledClassRow
        studentId="student-tomas"
        scheduledClass={buildStudentAppNextClass({
          date: tomorrow,
          classGroupId: "class-group-aquagym",
          isPackBooking: true,
        })}
        isToday={false}
      />,
    );

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.packClasses.cancel") }),
    );

    await waitFor(() =>
      expect(cancelPackClass).toHaveBeenCalledWith(
        "student-tomas",
        "class-group-aquagym",
        tomorrow,
      ),
    );
    expect(screen.queryByRole("button", { name: translate("student.absence.notify") })).toBeNull();
  });
});
