import { screen } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { getTeam } from "@/features/members/membersApi";
import { MemberScreen } from "@/features/members/screens/MemberScreen";
import { listRoles } from "@/features/roles/rolesApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSystemRoles } from "@/testing/roleFactory";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/roles/rolesApi");

const instructorOfMember = buildInstructor({ id: "instructor-of-member" });
const member = buildMember({ instructorId: instructorOfMember.id });

describe("When a member is linked to an instructor", () => {
  beforeEach(() => {
    jest.mocked(listRoles).mockResolvedValue(buildSystemRoles());
    jest.mocked(listActiveInstructors).mockResolvedValue([instructorOfMember]);
    jest.mocked(getTeam).mockResolvedValue({ members: [member], invitations: [] });
  });

  it("Then no instructor picker is shown", async () => {
    await renderWithProviders(<MemberScreen memberId={member.id} />);

    expect(
      await screen.findByText(translate("team.member.roleTitle", { name: member.fullName })),
    ).toBeOnTheScreen();
    expect(screen.queryByTestId("linked-instructor-picker")).toBeNull();
  });
});
