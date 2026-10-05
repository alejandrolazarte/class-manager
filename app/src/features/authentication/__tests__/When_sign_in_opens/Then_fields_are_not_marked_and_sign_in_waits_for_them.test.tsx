import { screen } from "@testing-library/react-native";
import { SignInScreen } from "@/features/authentication/screens/SignInScreen";
import { translate } from "@/i18n/translate";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

describe("When sign in opens", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
  });

  it("Then fields are not marked and sign in waits for them", async () => {
    await renderWithSession(<SignInScreen />);

    expect(
      await screen.findByRole("button", { name: translate("authentication.signIn.submit") }),
    ).toBeDisabled();
    expect(screen.queryByText(translate("common.requiredLegend"))).toBeNull();
    expect(screen.getByText(translate("authentication.signIn.email"))).toBeOnTheScreen();
  });
});
