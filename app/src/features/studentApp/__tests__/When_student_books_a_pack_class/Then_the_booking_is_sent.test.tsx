import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  bookPackClass,
  getStudentAppHome,
  getStudentAppPackClasses,
} from "@/features/studentApp/studentAppApi";
import { StudentAppClassesScreen } from "@/features/studentApp/screens/StudentAppClassesScreen";
import { todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import {
  buildStudentAppHome,
  buildStudentAppMakeupSlot,
  buildAccountStudent,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const today = todayIsoDate();

describe("When student books a pack class", () => {
  beforeEach(() => {
    jest.mocked(bookPackClass).mockResolvedValue();
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        students: [buildAccountStudent({ id: "student-tomas", nextClasses: [] })],
      }),
    );
    jest.mocked(getStudentAppPackClasses).mockResolvedValue({
      classesLeft: 4,
      slots: [buildStudentAppMakeupSlot({ classGroupId: "class-group-aquagym", date: today })],
    });
  });

  it("Then the booking is sent", async () => {
    await renderStudentAppScreen(<StudentAppClassesScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.makeup.book") }),
    );

    await waitFor(() =>
      expect(bookPackClass).toHaveBeenCalledWith("student-tomas", "class-group-aquagym", today),
    );
  });
});
