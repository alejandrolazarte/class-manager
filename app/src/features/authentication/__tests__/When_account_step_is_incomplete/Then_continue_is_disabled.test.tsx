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

  it("Then continue is disabled", async () => {
    await renderWithSession(<SignUpScreen />);

    await fireEvent.changeText(
      await screen.findByLabelText(translate("authentication.signUp.ownerFullName")),
      "Laura Gómez",
    );

    expect(screen.getByRole("button", { name: translate("common.continue") })).toBeDisabled();
  });
});
