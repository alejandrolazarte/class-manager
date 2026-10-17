import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { AddStudentScreen } from "@/features/students/screens/AddStudentScreen";
import { addStudent } from "@/features/students/studentsApi";
import { userErrorCodes } from "@/features/users/userErrorCodes";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/students/studentsApi");

const conflictStatus = 409;
const clientId = "0192f0c4-0000-7000-8000-000000000001";
const instructorUserId = "user-of-the-instructor";

describe("When a student is added with the email of an instructor", () => {
  beforeEach(() => {
    jest.mocked(addStudent).mockRejectedValueOnce(
      new ApiError(conflictStatus, {
        code: userErrorCodes.samePerson,
        samePersonUserId: instructorUserId,
        samePersonFullName: "Ana Gómez",
        samePersonField: "Email",
      }),
    );
  });

  it("Then it is added after confirming the same person", async () => {
    await renderWithProviders(<AddStudentScreen clientId={clientId} />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("students.fields.fullName")),
      "Ana Gómez",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("students.fields.email")),
      "ana@example.com",
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("students.add.submit") }));
    expect(
      await screen.findByText(translate("people.samePerson.question", { name: "Ana Gómez" })),
    ).toBeOnTheScreen();
    expect(addStudent).toHaveBeenCalledTimes(1);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("people.samePerson.confirm") }),
    );

    await waitFor(() =>
      expect(jest.mocked(addStudent).mock.calls[1]?.[1]).toEqual(
        expect.objectContaining({ email: "ana@example.com", samePersonUserId: instructorUserId }),
      ),
    );
  });
});
