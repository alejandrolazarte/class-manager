import { fireEvent, screen } from "@testing-library/react-native";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { InstructorDetailScreen } from "@/features/instructors/screens/InstructorDetailScreen";
import { getTeam } from "@/features/members/membersApi";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildInstructor } from "@/testing/classGroupFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { buildMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/members/membersApi");
jest.mock("@/features/classGroups/classGroupsApi");

const instructor = buildInstructor();
const member = buildMember({ instructorId: instructor.id });

describe("When an instructor is on the team", () => {
  beforeEach(() => {
    jest.mocked(listInstructorsIncludingInactive).mockResolvedValue([instructor]);
    jest.mocked(getTeam).mockResolvedValue({ members: [member], invitations: [] });
  });

  it("Then the role can be changed", async () => {
    await renderWithProviders(<InstructorDetailScreen instructorId={instructor.id} />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("team.member.changeRoleAccessibility", { name: member.fullName }),
      }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(routes.teamMember(member.id));
  });
});
