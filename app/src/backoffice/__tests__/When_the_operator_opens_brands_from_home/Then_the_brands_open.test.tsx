import { fireEvent, screen } from "@testing-library/react-native";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { BackofficeHomeScreen } from "@/backoffice/home/BackofficeHomeScreen";
import {
  getCurrentOperator,
  refreshBackofficeSession,
} from "@/backoffice/session/backofficeSessionApi";
import { translate } from "@/i18n/translate";
import { routerMock } from "@/testing/expoRouterMock";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/session/backofficeSessionApi");

describe("When the operator opens brands from home", () => {
  beforeEach(() => {
    jest.mocked(refreshBackofficeSession).mockResolvedValue("access-token");
    jest
      .mocked(getCurrentOperator)
      .mockResolvedValue({ email: "operator@example.com", role: "PlatformReader" });
  });

  it("Then the brands open", async () => {
    await renderBackofficeScreen(<BackofficeHomeScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("backoffice.home.brands") }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(backofficeRoutes.brands);
  });
});
