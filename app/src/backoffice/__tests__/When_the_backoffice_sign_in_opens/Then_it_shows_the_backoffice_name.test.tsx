import { screen } from "@testing-library/react-native";
import { BackofficeSignInScreen } from "@/backoffice/signIn/BackofficeSignInScreen";
import { translate } from "@/i18n/translate";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

describe("When the backoffice sign in opens", () => {
  it("Then it shows the backoffice name", async () => {
    await renderBackofficeScreen(<BackofficeSignInScreen />);

    expect(
      screen.getByRole("header", { name: translate("backoffice.signIn.title") }),
    ).toBeOnTheScreen();
  });
});
