import { fireEvent, screen } from "@testing-library/react-native";
import { createInstructor } from "@/features/instructors/instructorsApi";
import { InstructorFormScreen } from "@/features/instructors/screens/InstructorFormScreen";
import { getTeam } from "@/features/members/membersApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSamePersonError } from "@/testing/samePersonError";

jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/members/membersApi");

const studentUserId = "0192f0c4-0000-7000-8000-000000000010";

describe("When instructor is created with the email of a student", () => {
  beforeEach(() => {
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [] });
    jest
      .mocked(createInstructor)
      .mockRejectedValue(buildSamePersonError(studentUserId, "Laura Gómez"));
  });

  it("Then nothing is saved until it is confirmed", async () => {
    await renderWithProviders(<InstructorFormScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("instructors.form.fullName")),
      "Laura Gómez",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("instructors.form.email")),
      "laura@example.com",
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    expect(
      await screen.findByText(translate("people.samePerson.question", { name: "Laura Gómez" })),
    ).toBeOnTheScreen();
    await fireEvent.press(screen.getAllByRole("button", { name: translate("common.back") })[0]);
    expect(createInstructor).toHaveBeenCalledTimes(1);
    expect(await screen.findByText(translate("apiErrors.user.email_taken"))).toBeOnTheScreen();
  });
});
