import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createRole, deleteRole, listRoles } from "@/features/roles/rolesApi";
import { RoleEditorScreen } from "@/features/roles/screens/RoleEditorScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildCustomRole } from "@/testing/roleFactory";

jest.mock("@/features/roles/rolesApi");

const role = buildCustomRole();

describe("When a deleted role is undone", () => {
  beforeEach(() => {
    jest.mocked(listRoles).mockResolvedValue([role]);
    jest.mocked(deleteRole).mockResolvedValue(undefined);
    jest.mocked(createRole).mockResolvedValue(role);
  });

  it("Then it is created again", async () => {
    await renderWithProviders(<RoleEditorScreen roleKey={role.id!} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("roles.editor.delete") }),
    );

    await fireEvent.press(await screen.findByRole("button", { name: translate("common.undo") }));

    await waitFor(() =>
      expect(createRole).toHaveBeenCalledWith({
        name: role.name,
        permissions: role.permissions,
        copiedFrom: role.copiedFrom,
      }),
    );
  });
});
