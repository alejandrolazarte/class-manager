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
const fullName = "Marcos Díaz";
const password = "a long passphrase";

describe("When invitation is accepted", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: invitationToken };
    jest.mocked(checkInvitation).mockResolvedValue(buildCheckedInvitation());
    jest.mocked(acceptInvitation).mockResolvedValue(buildTokenResponse());
  });

  it("Then session starts and today opens", async () => {
    await renderWithSession(<AcceptInvitationScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("invitation.fullName")),
      fullName,
    );
    await fireEvent.changeText(screen.getByLabelText(translate("invitation.password")), password);
    await fireEvent.changeText(
      screen.getByLabelText(translate("invitationAnswer.passwordConfirmation")),
      password,
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("invitation.submit") }));

    await waitFor(() => expect(routerMock.replace).toHaveBeenCalledWith(routes.today));
    expect(acceptInvitation).toHaveBeenCalledWith({ token: invitationToken, fullName, password });
  });
});
