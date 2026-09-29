import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { getTeam, makeBrandOwner } from "@/features/members/membersApi";
import { MemberScreen } from "@/features/members/screens/MemberScreen";
import { translate } from "@/i18n/translate";
import { buildMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");

const branchOwner = buildMember({ role: "BranchOwner", instructorId: null });

describe("When member is made brand owner", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([]);
    jest.mocked(getTeam).mockResolvedValue({ members: [branchOwner], invitations: [] });
    jest.mocked(makeBrandOwner).mockResolvedValue(undefined);
  });

  it("Then request is sent", async () => {
    await renderWithProviders(<MemberScreen memberId={branchOwner.id} />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("team.member.makeBrandOwner") }),
    );

    await waitFor(() => expect(makeBrandOwner).toHaveBeenCalledWith(branchOwner.id));
  });
});
