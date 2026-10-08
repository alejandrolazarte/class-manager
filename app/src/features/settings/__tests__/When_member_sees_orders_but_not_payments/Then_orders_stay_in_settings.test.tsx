import { screen } from "@testing-library/react-native";
import { listBranches } from "@/features/branches/branchesApi";
import { SettingsScreen } from "@/features/settings/screens/SettingsScreen";
import { translate } from "@/i18n/translate";
import { buildCurrentMember, instructorPermissions } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/branches/branchesApi");
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({ signOut: jest.fn() }),
}));

describe("When member sees orders but not payments", () => {
  beforeEach(() => {
    jest.mocked(listBranches).mockResolvedValue([]);
  });

  it("Then orders stay in settings", async () => {
    await renderWithProviders(<SettingsScreen />, {
      member: buildCurrentMember({ permissions: [...instructorPermissions, "orders.view.own"] }),
    });

    expect(screen.getByRole("button", { name: translate("settings.orders") })).toBeOnTheScreen();
  });
});
