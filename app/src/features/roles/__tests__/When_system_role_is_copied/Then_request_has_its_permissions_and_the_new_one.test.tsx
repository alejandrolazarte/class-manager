import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createRole, listRoles } from "@/features/roles/rolesApi";
import { RoleEditorScreen } from "@/features/roles/screens/RoleEditorScreen";
import { translate } from "@/i18n/translate";
import { routerMock } from "@/testing/expoRouterMock";
import { coachPermissions } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildCustomRole, buildSystemRoles } from "@/testing/roleFactory";

jest.mock("@/features/roles/rolesApi");

describe("When system role is copied", () => {
  beforeEach(() => {
    jest.mocked(listRoles).mockResolvedValue(buildSystemRoles());
    jest.mocked(createRole).mockResolvedValue(buildCustomRole());
  });

  it("Then request has its permissions and the new one", async () => {
    await renderWithProviders(<RoleEditorScreen copyFromRoleKey="Coach" />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("roles.editor.name")),
      "Coach que cobra",
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("roles.permission.paymentsRecord") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(createRole).toHaveBeenCalledWith({
        name: "Coach que cobra",
        permissions: [...coachPermissions, "payments.record"],
        copiedFrom: "Coach",
      }),
    );
    expect(routerMock.back).toHaveBeenCalled();
  });
});
