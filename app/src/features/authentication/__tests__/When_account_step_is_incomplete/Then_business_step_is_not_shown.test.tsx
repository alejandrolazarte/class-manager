import { fireEvent, screen } from "@testing-library/react-native";
import { SignUpScreen } from "@/features/authentication/screens/SignUpScreen";
import { translate } from "@/i18n/translate";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

describe("When account step is incomplete", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
  });

  it("Then business step is not shown", async () => {
    await renderWithSession(<SignUpScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("common.continue") }),
    );

    expect(
      await screen.findByText(translate("authentication.validation.fullNameRequired")),
    ).toBeOnTheScreen();
    expect(
      screen.queryByLabelText(translate("authentication.signUp.businessName")),
    ).not.toBeOnTheScreen();
  });
});
