import { screen } from "@testing-library/react-native";
import { listBranches } from "@/features/branches/branchesApi";
import { SettingsScreen } from "@/features/settings/screens/SettingsScreen";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/branches/branchesApi");
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({ signOut: jest.fn() }),
}));

const statusTileClassNames = ["bg-success-soft", "bg-warning-soft"];

describe("When settings rows are shown", () => {
  beforeEach(() => {
    jest.mocked(listBranches).mockResolvedValue([]);
  });

  it("Then only brand colors decorate them", async () => {
    await renderWithProviders(<SettingsScreen />);

    const renderedTree = JSON.stringify(screen.toJSON());
    for (const statusTileClassName of statusTileClassNames) {
      expect(renderedTree).not.toContain(statusTileClassName);
    }
  });
});
