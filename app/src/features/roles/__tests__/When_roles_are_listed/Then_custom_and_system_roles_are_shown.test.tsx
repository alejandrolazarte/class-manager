import { screen } from "@testing-library/react-native";
import { listRoles } from "@/features/roles/rolesApi";
import { RolesScreen } from "@/features/roles/screens/RolesScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildCustomRole, buildSystemRoles } from "@/testing/roleFactory";

jest.mock("@/features/roles/rolesApi");

const customRole = buildCustomRole({ memberCount: 2 });

describe("When roles are listed", () => {
  beforeEach(() => {
    jest.mocked(listRoles).mockResolvedValue([...buildSystemRoles(), customRole]);
  });

  it("Then custom and system roles are shown", async () => {
    await renderWithProviders(<RolesScreen />);

    expect(await screen.findByRole("button", { name: customRole.name! })).toBeOnTheScreen();
    expect(screen.getByText(translate("roles.memberCount.other", { count: 2 }))).toBeOnTheScreen();
    expect(screen.getByRole("button", { name: translate("roles.Viewer") })).toBeOnTheScreen();
  });
});
