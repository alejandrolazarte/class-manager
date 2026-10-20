import { fireEvent, screen } from "@testing-library/react-native";
import { backofficeConfiguration } from "@/backoffice/backofficeConfiguration";
import { BackofficeSignInScreen } from "@/backoffice/signIn/BackofficeSignInScreen";
import { translate } from "@/i18n/translate";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

const googleSignInPath = "/api/auth/signin/google";

describe("When the operator chooses Google", () => {
  const assign = jest.fn();

  beforeEach(() => {
    Object.defineProperty(globalThis, "location", { value: { assign }, configurable: true });
  });

  it("Then the browser goes to the Google sign in", async () => {
    await renderBackofficeScreen(<BackofficeSignInScreen />);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("backoffice.signIn.withGoogle") }),
    );

    expect(assign).toHaveBeenCalledWith(backofficeConfiguration.apiBaseUrl + googleSignInPath);
  });
});
