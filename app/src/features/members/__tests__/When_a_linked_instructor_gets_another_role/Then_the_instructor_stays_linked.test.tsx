import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { changeMemberRole, getTeam } from "@/features/members/membersApi";
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

describe("When a linked instructor gets another role", () => {
  beforeEach(() => {
    jest.mocked(listRoles).mockResolvedValue(buildSystemRoles());
    jest.mocked(listActiveInstructors).mockResolvedValue([instructorOfMember]);
    jest.mocked(getTeam).mockResolvedValue({ members: [member], invitations: [] });
    jest.mocked(changeMemberRole).mockResolvedValue({ ...member, role: "Viewer" });
  });

  it("Then the instructor stays linked", async () => {
    await renderWithProviders(<MemberScreen memberId={member.id} />);
    await fireEvent.press(await screen.findByRole("button", { name: translate("roles.Viewer") }));

    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(changeMemberRole).toHaveBeenCalledWith(member.id, {
        role: "Viewer",
        customRoleId: null,
        instructorId: instructorOfMember.id,
      }),
    );
  });
});
