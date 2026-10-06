import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { InstructorDetailScreen } from "@/features/instructors/screens/InstructorDetailScreen";
import { getTeam, inviteMember } from "@/features/members/membersApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildInvitation } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/members/membersApi");

const instructor = buildInstructor();

describe("When instructor without email is invited", () => {
  beforeEach(() => {
    jest.mocked(listInstructorsIncludingInactive).mockResolvedValue([instructor]);
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [] });
    jest.mocked(inviteMember).mockResolvedValue(buildInvitation());
  });

  it("Then the typed email is sent", async () => {
    await renderWithProviders(<InstructorDetailScreen instructorId={instructor.id} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("instructors.app.inviteAccessibility") }),
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("instructors.invite.email")),
      " laura@example.com ",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("team.invite.submit") }));

    await waitFor(() =>
      expect(inviteMember).toHaveBeenCalledWith(
        expect.objectContaining({ email: "laura@example.com", instructorId: instructor.id }),
      ),
    );
  });
});
