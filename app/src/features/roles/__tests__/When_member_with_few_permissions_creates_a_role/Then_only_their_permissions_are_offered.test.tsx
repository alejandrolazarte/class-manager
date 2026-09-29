import { screen } from "@testing-library/react-native";
import { listRoles } from "@/features/roles/rolesApi";
import { RoleEditorScreen } from "@/features/roles/screens/RoleEditorScreen";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSystemRoles } from "@/testing/roleFactory";

jest.mock("@/features/roles/rolesApi");

describe("When member with few permissions creates a role", () => {
  beforeEach(() => {
    jest.mocked(listRoles).mockResolvedValue(buildSystemRoles());
  });

  it("Then only their permissions are offered", async () => {
    await renderWithProviders(<RoleEditorScreen />, {
      member: buildCurrentMember({
        branchRole: "Custom",
        isBrandOwner: false,
        permissions: ["roles.manage", "members.view", "students.view.own"],
      }),
    });

    expect(
      await screen.findByRole("button", {
        name: `${translate("roles.permission.studentsView")}: ${translate("roles.scope.own")}`,
      }),
    ).toBeOnTheScreen();
    expect(
      screen.queryByRole("button", {
        name: `${translate("roles.permission.studentsView")}: ${translate("roles.scope.all")}`,
      }),
    ).toBeNull();
    expect(
      screen.queryByRole("button", { name: translate("roles.permission.paymentsRecord") }),
    ).toBeNull();
  });
});
