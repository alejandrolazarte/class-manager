import { screen } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { getTeam } from "@/features/members/membersApi";
import { everyPermission } from "@/features/members/permissions";
import { AddTeamMemberScreen } from "@/features/members/screens/AddTeamMemberScreen";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");

const branchOwner = buildCurrentMember({
  isBrandOwner: false,
  permissions: everyPermission.filter((permission) => permission !== "branchOwners.manage"),
});

describe("When member without brand permissions invites", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([]);
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [] });
  });

  it("Then branch owner role is not offered", async () => {
    await renderWithProviders(<AddTeamMemberScreen />, { member: branchOwner });

    expect(
      await screen.findByRole("button", { name: translate("roles.Viewer") }),
    ).toBeOnTheScreen();
    expect(screen.queryByRole("button", { name: translate("roles.BranchOwner") })).toBeNull();
  });
});
