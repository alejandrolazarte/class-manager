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

const instructor = buildInstructor({ email: "laura@example.com" });

describe("When instructor with an email is invited", () => {
  beforeEach(() => {
    jest.mocked(listInstructorsIncludingInactive).mockResolvedValue([instructor]);
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [] });
    jest
      .mocked(inviteMember)
      .mockResolvedValue(
        buildInvitation({ email: "laura@example.com", role: "Coach", instructorId: instructor.id }),
      );
  });

  it("Then request has the coach role and instructor", async () => {
    await renderWithProviders(<InstructorDetailScreen instructorId={instructor.id} />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("instructors.app.inviteAccessibility") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("team.invite.submit") }));

    await waitFor(() =>
      expect(inviteMember).toHaveBeenCalledWith({
        email: "laura@example.com",
        role: "Coach",
        customRoleId: null,
        instructorId: instructor.id,
      }),
    );
  });
});
