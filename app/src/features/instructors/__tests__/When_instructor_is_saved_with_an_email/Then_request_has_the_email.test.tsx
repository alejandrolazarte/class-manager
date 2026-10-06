import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createInstructor } from "@/features/instructors/instructorsApi";
import { InstructorFormScreen } from "@/features/instructors/screens/InstructorFormScreen";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/instructors/instructorsApi");

describe("When instructor is saved with an email", () => {
  beforeEach(() => {
    jest.mocked(createInstructor).mockResolvedValue(buildInstructor());
  });

  it("Then request has the email", async () => {
    await renderWithProviders(<InstructorFormScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("instructors.form.fullName")),
      "Laura Gómez",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("instructors.form.email")),
      " laura@example.com ",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(createInstructor).toHaveBeenCalledWith({
        fullName: "Laura Gómez",
        email: "laura@example.com",
      }),
    );
  });
});
