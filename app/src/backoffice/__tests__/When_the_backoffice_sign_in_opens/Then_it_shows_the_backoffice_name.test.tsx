import { render, screen } from "@testing-library/react-native";
import { BackofficeSignInScreen } from "@/backoffice/signIn/BackofficeSignInScreen";
import { translate } from "@/i18n/translate";
import { ThemeProvider } from "@/theme/ThemeProvider";

describe("When the backoffice sign in opens", () => {
  it("Then it shows the backoffice name", async () => {
    await render(
      <ThemeProvider>
        <BackofficeSignInScreen />
      </ThemeProvider>,
    );

    expect(
      screen.getByRole("header", { name: translate("backoffice.signIn.title") }),
    ).toBeOnTheScreen();
  });
});
