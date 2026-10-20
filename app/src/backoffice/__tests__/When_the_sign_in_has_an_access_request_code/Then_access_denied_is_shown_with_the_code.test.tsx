import { screen } from "@testing-library/react-native";
import { BackofficeSignInScreen } from "@/backoffice/signIn/BackofficeSignInScreen";
import { translate } from "@/i18n/translate";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

const accessRequestCode = "K7-3QF";

describe("When the sign in has an access request code", () => {
  beforeEach(() => {
    searchParametersMock.current = { accessRequestCode };
  });

  it("Then access denied is shown with the code", async () => {
    await renderBackofficeScreen(<BackofficeSignInScreen />);

    expect(screen.getByText(translate("backoffice.signIn.accessDenied"))).toBeOnTheScreen();
    expect(screen.getByText(accessRequestCode)).toBeOnTheScreen();
  });
});
