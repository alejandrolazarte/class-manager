import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createInstructor, listActiveInstructors } from "@/features/instructors/instructorsApi";
import { getTeam, inviteMember } from "@/features/members/membersApi";
import { AddTeamMemberScreen } from "@/features/members/screens/AddTeamMemberScreen";
import { listRoles } from "@/features/roles/rolesApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSystemRoles } from "@/testing/roleFactory";

jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/members/membersApi");
jest.mock("@/features/roles/rolesApi");

describe("When an instructor is added without an email", () => {
  beforeEach(() => {
    jest.mocked(listRoles).mockResolvedValue(buildSystemRoles());
    jest.mocked(listActiveInstructors).mockResolvedValue([]);
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [] });
    jest.mocked(createInstructor).mockResolvedValue(buildInstructor());
  });

  it("Then no invitation is sent", async () => {
    await renderWithProviders(<AddTeamMemberScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("instructors.form.fullName")),
      "Laura Gómez",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(createInstructor).toHaveBeenCalledWith({ fullName: "Laura Gómez", email: null }),
    );
    expect(routerMock.back).toHaveBeenCalled();
    expect(inviteMember).not.toHaveBeenCalled();
  });
});
