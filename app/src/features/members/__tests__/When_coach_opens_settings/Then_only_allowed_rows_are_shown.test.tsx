import { screen } from "@testing-library/react-native";
import { listBranches } from "@/features/branches/branchesApi";
import { SettingsScreen } from "@/features/settings/screens/SettingsScreen";
import { translate } from "@/i18n/translate";
import { buildCoach } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/branches/branchesApi");
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({ signOut: jest.fn() }),
}));

describe("When coach opens settings", () => {
  beforeEach(() => {
    jest.mocked(listBranches).mockResolvedValue([]);
  });

  it("Then only allowed rows are shown", async () => {
    await renderWithProviders(<SettingsScreen />, { member: buildCoach() });

    expect(screen.getByText(translate("settings.instructors"))).toBeOnTheScreen();
    expect(screen.queryByText(translate("settings.monthlyFee"))).toBeNull();
    expect(screen.queryByText(translate("settings.classPacks"))).toBeNull();
    expect(screen.queryByText(translate("settings.importExport"))).toBeNull();
    expect(screen.queryByText(translate("settings.team"))).toBeNull();
    expect(screen.queryByText(translate("settings.branches"))).toBeNull();
    expect(screen.queryByRole("button", { name: translate("settings.business") })).toBeNull();
  });
});
