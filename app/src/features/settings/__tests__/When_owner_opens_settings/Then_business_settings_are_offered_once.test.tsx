import { screen } from "@testing-library/react-native";
import { listBranches } from "@/features/branches/branchesApi";
import { SettingsScreen } from "@/features/settings/screens/SettingsScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/branches/branchesApi");
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({ signOut: jest.fn() }),
}));

describe("When owner opens settings", () => {
  beforeEach(() => {
    jest.mocked(listBranches).mockResolvedValue([]);
  });

  it("Then business settings are offered once", async () => {
    await renderWithProviders(<SettingsScreen />);

    expect(screen.getAllByRole("button", { name: translate("settings.business") })).toHaveLength(1);
  });
});
