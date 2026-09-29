import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { acceptFamilyInvitation } from "@/features/authentication/authenticationApi";
import { AcceptFamilyInvitationScreen } from "@/features/family/screens/AcceptFamilyInvitationScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildTokenResponse } from "@/testing/authenticationFactory";
import { routerMock, searchParametersMock } from "@/testing/expoRouterMock";
import { buildFamilyAccessToken } from "@/testing/familyFactory";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const invitationToken = "family-invitation-token";
const fullName = "Ana Pérez";
const password = "a long family passphrase";

describe("When family invitation is accepted", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: invitationToken };
    jest
      .mocked(acceptFamilyInvitation)
      .mockResolvedValue(buildTokenResponse({ accessToken: buildFamilyAccessToken() }));
  });

  it("Then family home opens", async () => {
    await renderWithSession(<AcceptFamilyInvitationScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("familyInvitation.fullName")),
      fullName,
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("familyInvitation.password")),
      password,
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("familyInvitation.submit") }),
    );

    await waitFor(() => expect(routerMock.replace).toHaveBeenCalledWith(routes.family));
    expect(acceptFamilyInvitation).toHaveBeenCalledWith({
      token: invitationToken,
      fullName,
      password,
    });
  });
});
