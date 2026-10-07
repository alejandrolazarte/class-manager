import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { getTeam, removeBrandOwner } from "@/features/members/membersApi";
import { MemberScreen } from "@/features/members/screens/MemberScreen";
import { translate } from "@/i18n/translate";
import { buildMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");

const brandOwner = buildMember({ role: "BranchOwner", instructorId: null, isBrandOwner: true });

describe("When a brand owner is removed", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([]);
    jest.mocked(getTeam).mockResolvedValue({ members: [brandOwner], invitations: [] });
    jest.mocked(removeBrandOwner).mockResolvedValue(undefined);
  });

  it("Then it is removed after confirming", async () => {
    await renderWithProviders(<MemberScreen memberId={brandOwner.id} />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("team.member.removeBrandOwner") }),
    );
    expect(removeBrandOwner).not.toHaveBeenCalled();
    await fireEvent.press(
      screen.getByRole("button", { name: translate("team.member.confirmRemoveBrandOwner") }),
    );

    await waitFor(() => expect(removeBrandOwner).toHaveBeenCalledWith(brandOwner.id));
  });
});
