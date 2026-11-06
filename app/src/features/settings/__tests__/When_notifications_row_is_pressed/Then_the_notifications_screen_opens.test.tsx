import { fireEvent, screen } from "@testing-library/react-native";
import { listBranches } from "@/features/branches/branchesApi";
import { SettingsScreen } from "@/features/settings/screens/SettingsScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/branches/branchesApi");
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({ signOut: jest.fn() }),
}));

describe("When notifications row is pressed", () => {
  beforeEach(() => {
    jest.mocked(listBranches).mockResolvedValue([]);
  });

  it("Then the notifications screen opens", async () => {
    await renderWithProviders(<SettingsScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.notifications.title") }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(routes.myNotificationPreferences);
  });
});
