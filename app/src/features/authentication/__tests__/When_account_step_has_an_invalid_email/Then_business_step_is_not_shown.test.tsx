import { fireEvent, screen } from "@testing-library/react-native";
import { SignUpScreen } from "@/features/authentication/screens/SignUpScreen";
import { translate } from "@/i18n/translate";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

describe("When account step has an invalid email", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
  });

  it("Then business step is not shown", async () => {
    await renderWithSession(<SignUpScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("authentication.signUp.ownerFullName")),
      "Laura Gómez",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("authentication.signUp.email")),
      "laura@",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("authentication.signUp.password")),
      "a long passphrase",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("common.continue") }));

    expect(
      await screen.findByText(translate("authentication.validation.emailInvalid")),
    ).toBeOnTheScreen();
    expect(
      screen.queryByLabelText(translate("authentication.signUp.businessName")),
    ).not.toBeOnTheScreen();
  });
});
