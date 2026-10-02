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

  it("Then rows are grouped by topic", async () => {
    await renderWithProviders(<SettingsScreen />);

    for (const groupKey of [
      "settings.group.business",
      "settings.group.people",
      "settings.group.shop",
      "settings.group.device",
      "settings.group.account",
    ] as const) {
      expect(screen.getByText(translate(groupKey))).toBeOnTheScreen();
    }
  });
});
