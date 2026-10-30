import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { getTeam, inviteMember } from "@/features/members/membersApi";
import { AddTeamMemberScreen } from "@/features/members/screens/AddTeamMemberScreen";
import { translate } from "@/i18n/translate";
import { listRoles } from "@/features/roles/rolesApi";
import { buildInstructor } from "@/testing/classGroupFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { buildInvitation, buildMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSystemRoles } from "@/testing/roleFactory";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/roles/rolesApi");

const freeInstructor = buildInstructor({ id: "instructor-free", fullName: "Marcos Díaz" });
const linkedInstructor = buildInstructor({ id: "instructor-linked", fullName: "Marcos Gómez" });
const email = "marcos@example.com";

describe("When instructor is invited", () => {
  beforeEach(() => {
    jest.mocked(listRoles).mockResolvedValue(buildSystemRoles());
    jest.mocked(listActiveInstructors).mockResolvedValue([freeInstructor, linkedInstructor]);
    jest.mocked(getTeam).mockResolvedValue({
      members: [buildMember({ instructorId: linkedInstructor.id })],
      invitations: [],
    });
    jest.mocked(inviteMember).mockResolvedValue(buildInvitation({ email, role: "Instructor" }));
  });

  it("Then request has email role and instructor", async () => {
    await renderWithProviders(<AddTeamMemberScreen />);
    await fireEvent.changeText(screen.getByLabelText(translate("team.invite.email")), email);
    await fireEvent.changeText(
      screen.getByLabelText(translate("instructors.form.fullName")),
      "Marcos",
    );
    const freeInstructorSuggestion = await screen.findByRole("button", {
      name: translate("team.add.useExisting", { name: freeInstructor.fullName }),
    });
    expect(
      screen.queryByRole("button", {
        name: translate("team.add.useExisting", { name: linkedInstructor.fullName }),
      }),
    ).toBeNull();
    await fireEvent.press(freeInstructorSuggestion);

    await fireEvent.press(screen.getByRole("button", { name: translate("team.invite.submit") }));

    await waitFor(() =>
      expect(inviteMember).toHaveBeenCalledWith({
        email,
        role: "Instructor",
        customRoleId: null,
        instructorId: freeInstructor.id,
      }),
    );
    expect(routerMock.back).toHaveBeenCalled();
  });
});
