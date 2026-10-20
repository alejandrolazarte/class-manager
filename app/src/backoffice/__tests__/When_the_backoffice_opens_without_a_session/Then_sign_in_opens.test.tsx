import { waitFor } from "@testing-library/react-native";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { BackofficeHomeScreen } from "@/backoffice/home/BackofficeHomeScreen";
import { BackofficeSessionGate } from "@/backoffice/session/BackofficeSessionGate";
import { refreshBackofficeSession } from "@/backoffice/session/backofficeSessionApi";
import { routerMock } from "@/testing/expoRouterMock";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/session/backofficeSessionApi");
jest.mock("@/backoffice/brands/brandsApi");
jest.mock("@/backoffice/brands/expiringAddOnsApi");

describe("When the backoffice opens without a session", () => {
  beforeEach(() => {
    jest.mocked(refreshBackofficeSession).mockResolvedValue(null);
  });

  it("Then sign in opens", async () => {
    await renderBackofficeScreen(
      <BackofficeSessionGate>
        <BackofficeHomeScreen />
      </BackofficeSessionGate>,
    );

    await waitFor(() => expect(routerMock.replace).toHaveBeenCalledWith(backofficeRoutes.signIn));
  });
});
