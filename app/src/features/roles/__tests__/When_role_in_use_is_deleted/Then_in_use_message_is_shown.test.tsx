import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { roleErrorCodes } from "@/features/roles/roleErrorCodes";
import { deleteRole, listRoles } from "@/features/roles/rolesApi";
import { RoleEditorScreen } from "@/features/roles/screens/RoleEditorScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildCustomRole } from "@/testing/roleFactory";

jest.mock("@/features/roles/rolesApi");

const conflictStatus = 409;
const role = buildCustomRole({ memberCount: 1 });

describe("When role in use is deleted", () => {
  beforeEach(() => {
    jest.mocked(listRoles).mockResolvedValue([role]);
    jest
      .mocked(deleteRole)
      .mockRejectedValue(new ApiError(conflictStatus, { code: roleErrorCodes.inUse }));
  });

  it("Then in use message is shown", async () => {
    await renderWithProviders(<RoleEditorScreen roleKey={role.id!} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("roles.editor.delete") }),
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("roles.editor.confirmDelete") }),
    );

    expect(await screen.findByText(translate("roles.editor.inUse"))).toBeOnTheScreen();
  });
});
