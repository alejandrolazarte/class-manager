import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { InstructorDetailScreen } from "@/features/instructors/screens/InstructorDetailScreen";
import { getTeam, revokeInvitation } from "@/features/members/membersApi";
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

describe("When the invitation of an instructor is revoked", () => {
  beforeEach(() => {
    jest.mocked(listInstructorsIncludingInactive).mockResolvedValue([instructor]);
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [invitation] });
    jest.mocked(revokeInvitation).mockResolvedValue(undefined);
  });

  it("Then request has the invitation", async () => {
    await renderWithProviders(<InstructorDetailScreen instructorId={instructor.id} />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: `${translate("team.revoke")} ${invitation.email}`,
      }),
    );

    await waitFor(() => expect(revokeInvitation).toHaveBeenCalledWith(invitation.id));
    expect(await screen.findByText(translate("team.revoked"))).toBeOnTheScreen();
  });
});
