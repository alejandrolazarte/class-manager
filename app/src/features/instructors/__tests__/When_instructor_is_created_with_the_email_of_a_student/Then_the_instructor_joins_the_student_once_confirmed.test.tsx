import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createInstructor } from "@/features/instructors/instructorsApi";
import { InstructorFormScreen } from "@/features/instructors/screens/InstructorFormScreen";
import { getTeam, inviteMember } from "@/features/members/membersApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildInvitation } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSamePersonError } from "@/testing/samePersonError";

jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/members/membersApi");

const studentUserId = "0192f0c4-0000-7000-8000-000000000010";
const instructor = buildInstructor({ fullName: "Laura Gómez", email: "laura@example.com" });

describe("When instructor is created with the email of a student", () => {
  beforeEach(() => {
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [] });
    jest.mocked(inviteMember).mockResolvedValue(buildInvitation({ instructorId: instructor.id }));
    jest
      .mocked(createInstructor)
      .mockRejectedValueOnce(buildSamePersonError(studentUserId, "Laura Gómez", ["Student"]))
      .mockResolvedValueOnce(instructor);
  });

  it("Then the instructor joins the student once confirmed", async () => {
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

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("people.samePerson.confirm") }),
    );

    await waitFor(() =>
      expect(createInstructor).toHaveBeenLastCalledWith(
        expect.objectContaining({ email: "laura@example.com", samePersonUserId: studentUserId }),
      ),
    );
  });
});
