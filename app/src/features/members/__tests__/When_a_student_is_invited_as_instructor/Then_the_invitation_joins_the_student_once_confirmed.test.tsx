import { fireEvent, screen, waitFor, within } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { getTeam, inviteMember } from "@/features/members/membersApi";
import { InviteMemberScreen } from "@/features/members/screens/InviteMemberScreen";
import { listRoles } from "@/features/roles/rolesApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildInvitation } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSystemRoles } from "@/testing/roleFactory";
import { buildSamePersonError } from "@/testing/samePersonError";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/roles/rolesApi");

const studentUserId = "0192f0c4-0000-7000-8000-000000000010";
const freeInstructor = buildInstructor({ id: "instructor-free", fullName: "Marcos Díaz" });

describe("When a student is invited as instructor", () => {
  beforeEach(() => {
    jest.mocked(listRoles).mockResolvedValue(buildSystemRoles());
    jest.mocked(listActiveInstructors).mockResolvedValue([freeInstructor]);
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [] });
    jest
      .mocked(inviteMember)
      .mockRejectedValueOnce(buildSamePersonError(studentUserId, "Marcos Díaz", ["Student"]))
      .mockResolvedValueOnce(buildInvitation({ instructorId: freeInstructor.id }));
  });

  it("Then the invitation joins the student once confirmed", async () => {
    await renderWithProviders(<InviteMemberScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("team.invite.email")),
      "marcos@example.com",
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
      await screen.findByRole("button", { name: translate("people.samePerson.confirm") }),
    );

    await waitFor(() =>
      expect(inviteMember).toHaveBeenLastCalledWith(
        expect.objectContaining({
          email: "marcos@example.com",
          instructorId: freeInstructor.id,
          samePersonUserId: studentUserId,
        }),
      ),
    );
  });
});
