import { screen } from "@testing-library/react-native";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { getTeam } from "@/features/members/membersApi";
import { TeamScreen } from "@/features/members/screens/TeamScreen";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildInvitation, buildMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");

const coachInstructor = buildInstructor({ id: "instructor-coach", fullName: "Marcos Díaz" });
const owner = buildMember({
  id: "member-owner",
  fullName: "Laura Gómez",
  email: "laura@example.com",
  role: "BranchOwner",
  instructorId: null,
  isCurrentUser: true,
});
const coach = buildMember({ instructorId: coachInstructor.id });
const invitation = buildInvitation();

describe("When team is listed", () => {
  beforeEach(() => {
    jest.mocked(getTeam).mockResolvedValue({ members: [owner, coach], invitations: [invitation] });
    jest.mocked(listInstructorsIncludingInactive).mockResolvedValue([coachInstructor]);
  });

  it("Then members and invitations are shown", async () => {
    await renderWithProviders(<TeamScreen />);

    expect(await screen.findByText(owner.fullName)).toBeOnTheScreen();
    expect(screen.getByText(translate("team.you"))).toBeOnTheScreen();
    expect(
      await screen.findByText(translate("roles.coachOf", { coach: coachInstructor.fullName })),
    ).toBeOnTheScreen();
    expect(screen.getByText(invitation.email)).toBeOnTheScreen();
    expect(
      screen.getByRole("button", { name: `${translate("team.revoke")} ${invitation.email}` }),
    ).toBeOnTheScreen();
  });
});
