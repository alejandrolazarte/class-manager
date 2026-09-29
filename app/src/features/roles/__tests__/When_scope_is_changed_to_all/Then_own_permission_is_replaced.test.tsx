import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listRoles, updateRole } from "@/features/roles/rolesApi";
import { RoleEditorScreen } from "@/features/roles/screens/RoleEditorScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildCustomRole } from "@/testing/roleFactory";

jest.mock("@/features/roles/rolesApi");

const role = buildCustomRole({ permissions: ["business.view", "sessions.view.own"] });

describe("When scope is changed to all", () => {
  beforeEach(() => {
    jest.mocked(listRoles).mockResolvedValue([role]);
    jest.mocked(updateRole).mockResolvedValue(role);
  });

  it("Then own permission is replaced", async () => {
    await renderWithProviders(<RoleEditorScreen roleKey={role.id!} />);
    const sessionsLabel = translate("roles.permission.sessionsView");

    await fireEvent.press(
      await screen.findByRole("button", {
        name: `${sessionsLabel}: ${translate("roles.scope.all")}`,
      }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(updateRole).toHaveBeenCalledWith(role.id, {
        name: role.name,
        permissions: ["business.view", "sessions.view.all"],
      }),
    );
  });
});
