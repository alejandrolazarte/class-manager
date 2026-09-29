import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { changeMemberRole, getTeam } from "@/features/members/membersApi";
import { MemberScreen } from "@/features/members/screens/MemberScreen";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { buildMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");

const coachInstructor = buildInstructor({ id: "instructor-coach" });
const coach = buildMember({ instructorId: coachInstructor.id });

describe("When member role is changed", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([coachInstructor]);
    jest.mocked(getTeam).mockResolvedValue({ members: [coach], invitations: [] });
    jest
      .mocked(changeMemberRole)
      .mockResolvedValue({ ...coach, role: "Viewer", instructorId: null });
  });

  it("Then request has new role", async () => {
    await renderWithProviders(<MemberScreen memberId={coach.id} />);
    await fireEvent.press(await screen.findByRole("button", { name: translate("roles.Viewer") }));

    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(changeMemberRole).toHaveBeenCalledWith(coach.id, {
        role: "Viewer",
        instructorId: null,
      }),
    );
    expect(routerMock.back).toHaveBeenCalled();
  });
});
