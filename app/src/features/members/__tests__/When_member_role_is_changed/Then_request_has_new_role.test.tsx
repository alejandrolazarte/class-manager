import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { changeMemberRole, getTeam } from "@/features/members/membersApi";
import { MemberScreen } from "@/features/members/screens/MemberScreen";
import { translate } from "@/i18n/translate";
import { listRoles } from "@/features/roles/rolesApi";
import { buildInstructor } from "@/testing/classGroupFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { buildMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSystemRoles } from "@/testing/roleFactory";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/roles/rolesApi");

const instructorOfMember = buildInstructor({ id: "instructor-of-member" });
const instructor = buildMember({ instructorId: instructorOfMember.id });

describe("When member role is changed", () => {
  beforeEach(() => {
    jest.mocked(listRoles).mockResolvedValue(buildSystemRoles());
    jest.mocked(listActiveInstructors).mockResolvedValue([instructorOfMember]);
    jest.mocked(getTeam).mockResolvedValue({ members: [instructor], invitations: [] });
    jest
      .mocked(changeMemberRole)
      .mockResolvedValue({ ...instructor, role: "Viewer", instructorId: null });
  });

  it("Then request has new role", async () => {
    await renderWithProviders(<MemberScreen memberId={instructor.id} />);
    await fireEvent.press(await screen.findByRole("button", { name: translate("roles.Viewer") }));

    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(changeMemberRole).toHaveBeenCalledWith(instructor.id, {
        role: "Viewer",
        customRoleId: null,
        instructorId: null,
      }),
    );
    expect(routerMock.back).toHaveBeenCalled();
  });
});
