import { fireEvent, screen, within } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { getTeam, inviteMember } from "@/features/members/membersApi";
import { InviteMemberScreen } from "@/features/members/screens/InviteMemberScreen";
import { listRoles } from "@/features/roles/rolesApi";
import { userErrorCodes } from "@/features/users/userErrorCodes";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSystemRoles } from "@/testing/roleFactory";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/roles/rolesApi");

const conflictStatus = 409;
const freeInstructor = buildInstructor({ id: "instructor-free", fullName: "Martina Gómez" });

describe("When the same person question is answered with back", () => {
  beforeEach(() => {
    jest.mocked(listRoles).mockResolvedValue(buildSystemRoles());
    jest.mocked(listActiveInstructors).mockResolvedValue([freeInstructor]);
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [] });
    jest.mocked(inviteMember).mockRejectedValue(
      new ApiError(conflictStatus, {
        code: userErrorCodes.samePerson,
        samePersonUserId: "user-of-the-student",
        samePersonFullName: "Ana Gómez",
      }),
    );
  });

  it("Then the email error is shown", async () => {
    await renderWithProviders(<InviteMemberScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("team.invite.email")),
      "ana@example.com",
    );
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("instructors.picker.choose") }),
    );
    await fireEvent.press(
      within(screen.getByTestId("linked-instructor-picker")).getByRole("button", {
        name: freeInstructor.fullName,
      }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("team.invite.submit") }));

    await fireEvent.press(
      (await screen.findAllByRole("button", { name: translate("common.back") }))[1]!,
    );

    expect(await screen.findByText(translate("apiErrors.user.email_taken"))).toBeOnTheScreen();
    expect(inviteMember).toHaveBeenCalledTimes(1);
  });
});
