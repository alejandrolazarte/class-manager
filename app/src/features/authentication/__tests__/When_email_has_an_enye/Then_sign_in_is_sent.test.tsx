import { waitFor } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { signIn } from "@/features/authentication/authenticationApi";
import { authenticationErrorCodes } from "@/features/authentication/authenticationErrorCodes";
import { SignInScreen } from "@/features/authentication/screens/SignInScreen";
import { fillSignInForm, ownerPassword, submitSignInForm } from "@/testing/fillAuthenticationForms";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const unauthorizedStatus = 401;
const emailWithEnye = "laura+dueñadesede@example.com";

describe("When email has an enye", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    jest
      .mocked(signIn)
      .mockRejectedValue(
        new ApiError(unauthorizedStatus, { code: authenticationErrorCodes.invalidCredentials }),
      );
  });

  it("Then sign in is sent", async () => {
    await renderWithSession(<SignInScreen />);
    await fillSignInForm(emailWithEnye);
    await submitSignInForm();

    await waitFor(() =>
      expect(signIn).toHaveBeenCalledWith({ email: emailWithEnye, password: ownerPassword }),
    );
  });
});
