import { screen } from "@testing-library/react-native";
import { refreshSession } from "@/features/authentication/authenticationApi";
import { buildStudentAccessToken } from "@/testing/studentAppFactory";
import { buildTokenResponse, storedRefreshToken } from "@/testing/authenticationFactory";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";
import { SessionProbe } from "@/testing/SessionProbe";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

describe("When student session is restored", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(storedRefreshToken);
    jest
      .mocked(refreshSession)
      .mockResolvedValue(buildTokenResponse({ accessToken: buildStudentAccessToken() }));
  });

  it("Then session kind is student", async () => {
    await renderWithSession(<SessionProbe />);

    expect(await screen.findByText("signedIn")).toBeOnTheScreen();
    expect(screen.getByText("student")).toBeOnTheScreen();
  });
});
