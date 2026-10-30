import { fireEvent, screen } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { getTeam } from "@/features/members/membersApi";
import { MemberScreen } from "@/features/members/screens/MemberScreen";
import { listRoles } from "@/features/roles/rolesApi";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildInstructor } from "@/testing/classGroupFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { buildMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSystemRoles } from "@/testing/roleFactory";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/roles/rolesApi");

const instructorOfMember = buildInstructor({ id: "instructor-of-member" });
const member = buildMember({ instructorId: instructorOfMember.id });

describe("When a member is an instructor", () => {
  beforeEach(() => {
    jest.mocked(listRoles).mockResolvedValue(buildSystemRoles());
    jest.mocked(listActiveInstructors).mockResolvedValue([instructorOfMember]);
    jest.mocked(getTeam).mockResolvedValue({ members: [member], invitations: [] });
  });

  it("Then the instructor profile can be opened", async () => {
    await renderWithProviders(<MemberScreen memberId={member.id} />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("team.member.openInstructor") }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(routes.instructor(instructorOfMember.id));
  });
});
