import { screen } from "@testing-library/react-native";
import { listBranches } from "@/features/branches/branchesApi";
import { SettingsScreen } from "@/features/settings/screens/SettingsScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/branches/branchesApi");
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({ signOut: jest.fn() }),
}));

describe("When member can see payments", () => {
  beforeEach(() => {
    jest.mocked(listBranches).mockResolvedValue([]);
  });

  it("Then orders live in cobros", async () => {
    await renderWithProviders(<SettingsScreen />);

    expect(screen.queryByText(translate("settings.orders"))).toBeNull();
  });
});
