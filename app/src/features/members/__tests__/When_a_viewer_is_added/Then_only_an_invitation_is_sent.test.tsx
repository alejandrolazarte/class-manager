import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createInstructor, listActiveInstructors } from "@/features/instructors/instructorsApi";
import { getTeam, inviteMember } from "@/features/members/membersApi";
import { AddTeamMemberScreen } from "@/features/members/screens/AddTeamMemberScreen";
import { listRoles } from "@/features/roles/rolesApi";
import { translate } from "@/i18n/translate";
import { buildInvitation } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSystemRoles } from "@/testing/roleFactory";

jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/members/membersApi");
jest.mock("@/features/roles/rolesApi");

const email = "lucia@example.com";

describe("When a viewer is added", () => {
  beforeEach(() => {
    jest.mocked(listRoles).mockResolvedValue(buildSystemRoles());
    jest.mocked(listActiveInstructors).mockResolvedValue([]);
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [] });
    jest.mocked(inviteMember).mockResolvedValue(buildInvitation({ email }));
  });

  it("Then only an invitation is sent", async () => {
    await renderWithProviders(<AddTeamMemberScreen />);
    await fireEvent.press(await screen.findByRole("button", { name: translate("roles.Viewer") }));
    expect(screen.queryByLabelText(translate("instructors.form.fullName"))).toBeNull();
    await fireEvent.changeText(screen.getByLabelText(translate("team.invite.email")), email);

    await fireEvent.press(screen.getByRole("button", { name: translate("team.invite.submit") }));

    await waitFor(() =>
      expect(inviteMember).toHaveBeenCalledWith({
        email,
        role: "Viewer",
        customRoleId: null,
        instructorId: null,
      }),
    );
    expect(createInstructor).not.toHaveBeenCalled();
  });
});
