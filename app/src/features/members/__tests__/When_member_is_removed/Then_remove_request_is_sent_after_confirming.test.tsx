import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { getTeam, removeMember } from "@/features/members/membersApi";
import { MemberScreen } from "@/features/members/screens/MemberScreen";
import { translate } from "@/i18n/translate";
import { buildMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");

const viewer = buildMember({ role: "Viewer", instructorId: null });

describe("When member is removed", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([]);
    jest.mocked(getTeam).mockResolvedValue({ members: [viewer], invitations: [] });
    jest.mocked(removeMember).mockResolvedValue(undefined);
  });

  it("Then remove request is sent after confirming", async () => {
    await renderWithProviders(<MemberScreen memberId={viewer.id} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("team.member.remove") }),
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("team.member.confirmRemove") }),
    );

    await waitFor(() => expect(removeMember).toHaveBeenCalledWith(viewer.id));
  });
});
