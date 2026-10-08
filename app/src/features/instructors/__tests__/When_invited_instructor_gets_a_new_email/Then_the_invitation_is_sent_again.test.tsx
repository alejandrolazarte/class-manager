import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  listInstructorsIncludingInactive,
  renameInstructor,
} from "@/features/instructors/instructorsApi";
import { InstructorFormScreen } from "@/features/instructors/screens/InstructorFormScreen";
import { getTeam, inviteMember } from "@/features/members/membersApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildInvitation } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/members/membersApi");

const instructor = buildInstructor({ email: "laura@example.com" });
const invitation = buildInvitation({
  email: "laura@example.com",
  role: "Instructor",
  instructorId: instructor.id,
});

describe("When invited instructor gets a new email", () => {
  beforeEach(() => {
    jest.mocked(listInstructorsIncludingInactive).mockResolvedValue([instructor]);
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [invitation] });
    jest.mocked(renameInstructor).mockResolvedValue(instructor);
    jest.mocked(inviteMember).mockResolvedValue(invitation);
  });

  it("Then the invitation is sent again", async () => {
    await renderWithProviders(<InstructorFormScreen instructorId={instructor.id} />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("instructors.form.email")),
      "laura.nueva@example.com",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(inviteMember).toHaveBeenCalledWith({
        email: "laura.nueva@example.com",
        role: "Instructor",
        customRoleId: null,
        instructorId: instructor.id,
      }),
    );
  });
});
