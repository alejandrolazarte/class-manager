import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { getTeam, inviteMember } from "@/features/members/membersApi";
import { InviteMemberScreen } from "@/features/members/screens/InviteMemberScreen";
import { listRoles } from "@/features/roles/rolesApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildInvitation } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildCustomRole, buildSystemRoles } from "@/testing/roleFactory";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/roles/rolesApi");

const instructor = buildInstructor({ id: "instructor-free", fullName: "Marcos Díaz" });
const customRole = buildCustomRole();
const email = "marcos@example.com";

describe("When member is invited with a custom role", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([instructor]);
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [] });
    jest.mocked(listRoles).mockResolvedValue([...buildSystemRoles(), customRole]);
    jest
      .mocked(inviteMember)
      .mockResolvedValue(buildInvitation({ email, role: "Custom", customRoleId: customRole.id }));
  });

  it("Then request has the custom role and instructor", async () => {
    await renderWithProviders(<InviteMemberScreen />);
    await fireEvent.changeText(screen.getByLabelText(translate("team.invite.email")), email);
    await fireEvent.press(await screen.findByRole("button", { name: customRole.name! }));
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("instructors.picker.choose") }),
    );
    await fireEvent.press(await screen.findByRole("button", { name: instructor.fullName }));

    await fireEvent.press(screen.getByRole("button", { name: translate("team.invite.submit") }));

    await waitFor(() =>
      expect(inviteMember).toHaveBeenCalledWith({
        email,
        role: "Custom",
        customRoleId: customRole.id,
        instructorId: instructor.id,
      }),
    );
  });
});
