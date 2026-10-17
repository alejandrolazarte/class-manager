import { fireEvent, screen, waitFor, within } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { getTeam, inviteMember } from "@/features/members/membersApi";
import { InviteMemberScreen } from "@/features/members/screens/InviteMemberScreen";
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

const conflictStatus = 409;
const freeInstructor = buildInstructor({ id: "instructor-free", fullName: "Ana Gómez" });
const studentUserId = "user-of-the-student";

describe("When the invited email belongs to a student", () => {
  beforeEach(() => {
    jest.mocked(listRoles).mockResolvedValue(buildSystemRoles());
    jest.mocked(listActiveInstructors).mockResolvedValue([freeInstructor]);
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [] });
    jest
      .mocked(inviteMember)
      .mockRejectedValueOnce(
        new ApiError(conflictStatus, {
          code: userErrorCodes.samePerson,
          samePersonUserId: studentUserId,
          samePersonFullName: "Ana Gómez",
        }),
      )
      .mockResolvedValueOnce({
        id: "invitation-1",
        email: "ana@example.com",
        role: "Instructor",
        customRoleId: null,
        instructorId: freeInstructor.id,
        expiresAt: "2026-10-24T12:00:00Z",
      });
  });

  it("Then it is sent after confirming the same person", async () => {
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
    expect(
      await screen.findByText(translate("people.samePerson.question", { name: "Ana Gómez" })),
    ).toBeOnTheScreen();
    expect(inviteMember).toHaveBeenCalledTimes(1);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("people.samePerson.confirm") }),
    );

    await waitFor(() =>
      expect(inviteMember).toHaveBeenLastCalledWith(
        expect.objectContaining({ samePersonUserId: studentUserId }),
      ),
    );
    expect(routerMock.back).toHaveBeenCalled();
  });
});
