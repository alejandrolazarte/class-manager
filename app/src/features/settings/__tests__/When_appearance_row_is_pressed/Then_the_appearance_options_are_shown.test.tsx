import { fireEvent, screen } from "@testing-library/react-native";
import { listBranches } from "@/features/branches/branchesApi";
import { SettingsScreen } from "@/features/settings/screens/SettingsScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/branches/branchesApi");
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({ signOut: jest.fn() }),
}));

describe("When appearance row is pressed", () => {
  beforeEach(() => {
    jest.mocked(listBranches).mockResolvedValue([]);
  });

  it("Then the appearance options are shown", async () => {
    await renderWithProviders(<SettingsScreen />);
    expect(screen.queryByText(translate("settings.appearance.dark"))).toBeNull();

    await fireEvent.press(screen.getByRole("button", { name: translate("settings.appearance") }));

    expect(await screen.findByText(translate("settings.appearance.dark"))).toBeOnTheScreen();
  });
});
