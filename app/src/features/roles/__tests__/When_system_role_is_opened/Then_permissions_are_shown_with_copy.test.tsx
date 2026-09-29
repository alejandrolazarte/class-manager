import { fireEvent, screen } from "@testing-library/react-native";
import { listRoles } from "@/features/roles/rolesApi";
import { RoleScreen } from "@/features/roles/screens/RoleScreen";
import { translate } from "@/i18n/translate";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSystemRoles } from "@/testing/roleFactory";

jest.mock("@/features/roles/rolesApi");

describe("When system role is opened", () => {
  beforeEach(() => {
    jest.mocked(listRoles).mockResolvedValue(buildSystemRoles());
  });

  it("Then permissions are shown with copy", async () => {
    await renderWithProviders(<RoleScreen roleKey="Coach" />);

    expect(
      await screen.findByText(
        `${translate("roles.permission.sessionsView")} · ${translate("roles.scope.own")}`,
      ),
    ).toBeOnTheScreen();
    await fireEvent.press(screen.getByRole("button", { name: translate("roles.copy") }));
    expect(routerMock.push).toHaveBeenCalledWith("/settings/roles/new?copyFrom=Coach");
  });
});
