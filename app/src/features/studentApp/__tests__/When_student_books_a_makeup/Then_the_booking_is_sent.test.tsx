import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  bookMakeup,
  getStudentAppHome,
  getStudentAppMakeups,
} from "@/features/studentApp/studentAppApi";
import { StudentAppClassesScreen } from "@/features/studentApp/screens/StudentAppClassesScreen";
import { todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import {
  buildStudentAppHome,
  buildStudentAppMakeups,
  buildStudentAppMakeupSlot,
  buildAccountStudent,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const today = todayIsoDate();

describe("When student books a makeup", () => {
  beforeEach(() => {
    jest.mocked(bookMakeup).mockResolvedValue();
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        students: [buildAccountStudent({ id: "student-tomas", nextClasses: [] })],
      }),
    );
    jest.mocked(getStudentAppMakeups).mockResolvedValue(
      buildStudentAppMakeups({
        slots: [buildStudentAppMakeupSlot({ classGroupId: "class-group-2", date: today })],
      }),
    );
  });

  it("Then the booking is sent", async () => {
    await renderStudentAppScreen(<StudentAppClassesScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.makeup.book") }),
    );

    await waitFor(() =>
      expect(bookMakeup).toHaveBeenCalledWith("student-tomas", "class-group-2", today),
    );
  });
});
