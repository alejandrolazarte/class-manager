import { screen } from "@testing-library/react-native";
import { BackofficeHomeScreen } from "@/backoffice/home/BackofficeHomeScreen";
import {
  getCurrentOperator,
  refreshBackofficeSession,
} from "@/backoffice/session/backofficeSessionApi";
import { translate } from "@/i18n/translate";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/session/backofficeSessionApi");

const operator = { email: "operator@example.com", role: "PlatformOperator" };

describe("When the backoffice opens with a session", () => {
  beforeEach(() => {
    jest.mocked(refreshBackofficeSession).mockResolvedValue("access-token");
    jest.mocked(getCurrentOperator).mockResolvedValue(operator);
  });

  it("Then the operator is shown", async () => {
    await renderBackofficeScreen(<BackofficeHomeScreen />);

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
