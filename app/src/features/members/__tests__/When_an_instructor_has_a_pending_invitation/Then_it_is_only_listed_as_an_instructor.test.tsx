import { screen } from "@testing-library/react-native";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { getTeam } from "@/features/members/membersApi";
import { TeamScreen } from "@/features/members/screens/TeamScreen";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildInvitation } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");

const invitedInstructor = buildInstructor({ id: "invited", fullName: "Ana Ruiz" });
const instructorInvitation = buildInvitation({
  email: "ana@example.com",
  role: "Instructor",
  instructorId: invitedInstructor.id,
});
const viewerInvitation = buildInvitation({ id: "invitation-2", email: "lucia@example.com" });

describe("When an instructor has a pending invitation", () => {
  beforeEach(() => {
    jest.mocked(getTeam).mockResolvedValue({
      members: [],
      invitations: [instructorInvitation, viewerInvitation],
    });
    jest.mocked(listInstructorsIncludingInactive).mockResolvedValue([invitedInstructor]);
  });

  it("Then it is only listed as an instructor", async () => {
    await renderWithProviders(<TeamScreen />);

    expect(
      await screen.findByRole("button", { name: invitedInstructor.fullName }),
    ).toBeOnTheScreen();
    expect(screen.getByText(translate("team.invited"))).toBeOnTheScreen();
    expect(screen.getByText(viewerInvitation.email)).toBeOnTheScreen();
    expect(screen.queryByText(instructorInvitation.email)).toBeNull();
    expect(
      screen.queryByRole("button", {
        name: `${translate("team.revoke")} ${instructorInvitation.email}`,
      }),
    ).toBeNull();
  });
});
