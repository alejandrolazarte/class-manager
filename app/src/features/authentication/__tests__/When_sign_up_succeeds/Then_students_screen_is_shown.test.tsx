import { waitFor } from "@testing-library/react-native";
import { signUp } from "@/features/authentication/authenticationApi";
import { RedirectWhenSignedIn } from "@/features/authentication/components/RedirectWhenSignedIn";
import { SignUpScreen } from "@/features/authentication/screens/SignUpScreen";
import { refreshTokenStorage } from "@/features/authentication/sessionStorage";
import { buildTokenResponse } from "@/testing/authenticationFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { fillSignUpForm, submitSignUpForm } from "@/testing/fillAuthenticationForms";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const issuedTokens = buildTokenResponse();

describe("When sign up succeeds", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    jest.mocked(signUp).mockResolvedValue(issuedTokens);
  });

  it("Then students screen is shown", async () => {
    await renderWithSession(
      <RedirectWhenSignedIn>
        <SignUpScreen />
      </RedirectWhenSignedIn>,
    );
    await fillSignUpForm();
    await submitSignUpForm();

    await waitFor(() => expect(routerMock.replace).toHaveBeenCalledWith("/students"));
    expect(refreshTokenStorage.write).toHaveBeenCalledWith(issuedTokens.refreshToken);
  });
});
