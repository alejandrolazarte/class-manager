import { screen } from "@testing-library/react-native";
import { BackofficeHomeScreen } from "@/backoffice/home/BackofficeHomeScreen";
import { BackofficeSessionGate } from "@/backoffice/session/BackofficeSessionGate";
import {
  getCurrentOperator,
  refreshBackofficeSession,
} from "@/backoffice/session/backofficeSessionApi";
import { translate } from "@/i18n/translate";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/session/backofficeSessionApi");
jest.mock("@/backoffice/brands/brandsApi");
jest.mock("@/backoffice/brands/expiringAddOnsApi");

const operator = { email: "operator@example.com", role: "PlatformOperator" };

describe("When the backoffice opens with a session", () => {
  beforeEach(() => {
    jest.mocked(refreshBackofficeSession).mockResolvedValue("access-token");
    jest.mocked(getCurrentOperator).mockResolvedValue(operator);
  });

  it("Then the operator is shown", async () => {
    await renderBackofficeScreen(
      <BackofficeSessionGate>
        <BackofficeHomeScreen />
      </BackofficeSessionGate>,
    );

    expect(
      await screen.findByText(
        translate("backoffice.home.signedInAs", {
          email: operator.email,
          role: translate("roles.PlatformOperator"),
        }),
      ),
    ).toBeOnTheScreen();
    expect(getCurrentOperator).toHaveBeenCalledWith("access-token");
  });
});
