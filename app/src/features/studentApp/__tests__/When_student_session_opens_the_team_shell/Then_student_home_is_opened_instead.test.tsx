import { RequireSessionKind } from "@/features/authentication/components/RequireSessionKind";
import { routes } from "@/navigation/routes";
import { Text } from "react-native";
import { screen, waitFor } from "@testing-library/react-native";
import { refreshSession } from "@/features/authentication/authenticationApi";
import { routerMock } from "@/testing/expoRouterMock";
import { buildTokenResponse, storedRefreshToken } from "@/testing/authenticationFactory";
import { buildStudentAccessToken } from "@/testing/studentAppFactory";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

describe("When student session opens the team shell", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(storedRefreshToken);
    jest
      .mocked(refreshSession)
      .mockResolvedValue(buildTokenResponse({ accessToken: buildStudentAccessToken() }));
  });

  it("Then student home is opened instead", async () => {
    await renderWithSession(
      <RequireSessionKind kind="team">
        <Text>team shell</Text>
      </RequireSessionKind>,
    );

    await waitFor(() => expect(routerMock.replace).toHaveBeenCalledWith(routes.studentApp));
    expect(screen.queryByText("team shell")).toBeNull();
  });
});
