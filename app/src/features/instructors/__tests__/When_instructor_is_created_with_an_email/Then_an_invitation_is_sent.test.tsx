import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createInstructor } from "@/features/instructors/instructorsApi";
import { InstructorFormScreen } from "@/features/instructors/screens/InstructorFormScreen";
import { getTeam, inviteMember } from "@/features/members/membersApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildInvitation } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/members/membersApi");

const instructor = buildInstructor({ email: "laura@example.com" });

describe("When instructor is created with an email", () => {
  beforeEach(() => {
    jest.mocked(createInstructor).mockResolvedValue(instructor);
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [] });
    jest.mocked(inviteMember).mockResolvedValue(buildInvitation({ instructorId: instructor.id }));
  });

  it("Then an invitation is sent", async () => {
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

    await waitFor(() =>
      expect(inviteMember).toHaveBeenCalledWith({
        email: "laura@example.com",
        role: "Instructor",
        customRoleId: null,
        instructorId: instructor.id,
      }),
    );
  });
});
