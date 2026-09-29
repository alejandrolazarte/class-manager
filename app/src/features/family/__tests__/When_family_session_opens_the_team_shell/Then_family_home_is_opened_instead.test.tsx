import { RequireSessionKind } from "@/features/authentication/components/RequireSessionKind";
import { routes } from "@/navigation/routes";
import { Text } from "react-native";
import { screen, waitFor } from "@testing-library/react-native";
import { refreshSession } from "@/features/authentication/authenticationApi";
import { routerMock } from "@/testing/expoRouterMock";
import { buildTokenResponse, storedRefreshToken } from "@/testing/authenticationFactory";
import { buildFamilyAccessToken } from "@/testing/familyFactory";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

describe("When family session opens the team shell", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(storedRefreshToken);
    jest
      .mocked(refreshSession)
      .mockResolvedValue(buildTokenResponse({ accessToken: buildFamilyAccessToken() }));
  });

  it("Then family home is opened instead", async () => {
    await renderWithSession(
      <RequireSessionKind kind="team">
        <Text>team shell</Text>
      </RequireSessionKind>,
    );

    await waitFor(() => expect(routerMock.replace).toHaveBeenCalledWith(routes.family));
    expect(screen.queryByText("team shell")).toBeNull();
  });
});
