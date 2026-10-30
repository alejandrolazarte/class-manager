import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { getTeam, inviteMember } from "@/features/members/membersApi";
import { AddTeamMemberScreen } from "@/features/members/screens/AddTeamMemberScreen";
import { listRoles } from "@/features/roles/rolesApi";
import { userErrorCodes } from "@/features/users/userErrorCodes";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSystemRoles } from "@/testing/roleFactory";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/roles/rolesApi");

const badRequestStatus = 400;
const freeInstructor = buildInstructor({ id: "instructor-free", fullName: "Marcos Díaz" });

describe("When instructor is invited with the email of another person", () => {
  beforeEach(() => {
    jest.mocked(listRoles).mockResolvedValue(buildSystemRoles());
    jest.mocked(listActiveInstructors).mockResolvedValue([freeInstructor]);
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [] });
    jest
      .mocked(inviteMember)
      .mockRejectedValue(new ApiError(badRequestStatus, { code: userErrorCodes.emailTaken }));
  });

  it("Then the email error is shown", async () => {
    await renderWithProviders(<AddTeamMemberScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("team.invite.email")),
      "ana@example.com",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("instructors.form.fullName")),
      freeInstructor.fullName.split(" ")[0]!,
    );
    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("team.add.useExisting", { name: freeInstructor.fullName }),
      }),
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("team.invite.submit") }));

    expect(await screen.findByText(translate("apiErrors.user.email_taken"))).toBeOnTheScreen();
    expect(routerMock.back).not.toHaveBeenCalled();
  });
});
