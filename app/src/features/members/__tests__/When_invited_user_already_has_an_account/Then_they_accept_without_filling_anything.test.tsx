import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { acceptInvitation, checkInvitation } from "@/features/authentication/authenticationApi";
import { AcceptInvitationScreen } from "@/features/members/screens/AcceptInvitationScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildCheckedInvitation, buildTokenResponse } from "@/testing/authenticationFactory";
import { routerMock, searchParametersMock } from "@/testing/expoRouterMock";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const invitationToken = "invitation-token-from-the-link";

describe("When invited user already has an account", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: invitationToken };
    jest.mocked(checkInvitation).mockResolvedValue(buildCheckedInvitation({ hasAccount: true }));
    jest.mocked(acceptInvitation).mockResolvedValue(buildTokenResponse());
  });

  it("Then they accept without filling anything", async () => {
    await renderWithSession(<AcceptInvitationScreen />);
    const acceptButton = await screen.findByRole("button", {
      name: translate("invitationAnswer.accept"),
    });
    expect(screen.queryByLabelText(translate("invitation.password"))).not.toBeOnTheScreen();

    await fireEvent.press(acceptButton);

    await waitFor(() => expect(routerMock.replace).toHaveBeenCalledWith(routes.today));
    expect(acceptInvitation).toHaveBeenCalledWith({
      token: invitationToken,
      fullName: "",
      password: "",
    });
  });
});
