import { screen } from "@testing-library/react-native";
import { listBranches } from "@/features/branches/branchesApi";
import { BranchesScreen } from "@/features/branches/screens/BranchesScreen";
import { translate } from "@/i18n/translate";
import { buildBranch } from "@/testing/branchFactory";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSubscription } from "@/testing/subscriptionFactory";

jest.mock("@/features/branches/branchesApi");
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({ switchBranch: jest.fn() }),
}));

describe("When the branch limit is reached", () => {
  beforeEach(() => {
    jest.mocked(listBranches).mockResolvedValue([buildBranch({ isCurrent: true })]);
  });

  it("Then a new branch cannot be created", async () => {
    await renderWithProviders(<BranchesScreen />, {
      member: buildCurrentMember({ subscription: buildSubscription() }),
    });

    expect(await screen.findByText(translate("subscriptions.locked.branches"))).toBeTruthy();
    expect(screen.queryByRole("button", { name: translate("branches.new") })).toBeNull();
  });
});
