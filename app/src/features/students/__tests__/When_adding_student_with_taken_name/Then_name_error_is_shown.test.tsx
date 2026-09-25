import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { AddStudentScreen } from "@/features/students/screens/AddStudentScreen";
import { studentErrorCodes } from "@/features/students/studentErrorCodes";
import { addStudent } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/students/studentsApi");

const conflictStatus = 409;
const clientId = "0192f0c4-0000-7000-8000-000000000001";

describe("When adding student with taken name", () => {
  beforeEach(() => {
    jest
      .mocked(addStudent)
      .mockRejectedValue(
        new ApiError(conflictStatus, { code: studentErrorCodes.alreadyRegistered }),
      );
  });

  it("Then name error is shown", async () => {
    await renderWithProviders(<AddStudentScreen clientId={clientId} />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("students.fields.fullName")),
      "Tomás Pérez",
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("students.add.submit") }));

    expect(await screen.findByText(translate("students.add.alreadyRegistered"))).toBeOnTheScreen();
  });
});
