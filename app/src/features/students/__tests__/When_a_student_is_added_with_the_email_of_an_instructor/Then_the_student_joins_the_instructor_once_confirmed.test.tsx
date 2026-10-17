import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { AddStudentScreen } from "@/features/students/screens/AddStudentScreen";
import { addStudent } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSamePersonError } from "@/testing/samePersonError";
import { buildStudent } from "@/testing/studentFactory";

jest.mock("@/features/students/studentsApi");

const clientId = "0192f0c4-0000-7000-8000-000000000001";
const instructorUserId = "0192f0c4-0000-7000-8000-000000000010";

describe("When a student is added with the email of an instructor", () => {
  beforeEach(() => {
    jest
      .mocked(addStudent)
      .mockRejectedValueOnce(buildSamePersonError(instructorUserId, "Laura Gómez"))
      .mockResolvedValueOnce(buildStudent({ fullName: "Laura Gómez" }));
  });

  it("Then the student joins the instructor once confirmed", async () => {
    await renderWithProviders(<AddStudentScreen clientId={clientId} />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("students.fields.fullName")),
      "Laura Gómez",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("students.fields.email")),
      "laura@example.com",
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("students.add.submit") }));

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("people.samePerson.confirm") }),
    );

    await waitFor(() =>
      expect(addStudent).toHaveBeenLastCalledWith(
        clientId,
        expect.objectContaining({ email: "laura@example.com", samePersonUserId: instructorUserId }),
      ),
    );
  });
});
