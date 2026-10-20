import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { BackofficeHomeScreen } from "@/backoffice/home/BackofficeHomeScreen";
import { BackofficeSessionGate } from "@/backoffice/session/BackofficeSessionGate";
import {
  getCurrentOperator,
  refreshBackofficeSession,
  signOutOfBackoffice,
} from "@/backoffice/session/backofficeSessionApi";
import { translate } from "@/i18n/translate";
import { routerMock } from "@/testing/expoRouterMock";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/session/backofficeSessionApi");
jest.mock("@/backoffice/brands/brandsApi");
jest.mock("@/backoffice/brands/expiringAddOnsApi");

describe("When the operator signs out", () => {
  beforeEach(() => {
    jest.mocked(refreshBackofficeSession).mockResolvedValue("access-token");
    jest
      .mocked(getCurrentOperator)
      .mockResolvedValue({ email: "operator@example.com", role: "PlatformReader" });
    jest.mocked(signOutOfBackoffice).mockResolvedValue(undefined);
  });

  it("Then the session ends and sign in opens", async () => {
    await renderBackofficeScreen(
      <BackofficeSessionGate>
        <BackofficeHomeScreen />
      </BackofficeSessionGate>,
    );

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("backoffice.home.signOut") }),
    );

    expect(signOutOfBackoffice).toHaveBeenCalledTimes(1);
    await waitFor(() => expect(routerMock.replace).toHaveBeenCalledWith(backofficeRoutes.signIn));
  });
});
