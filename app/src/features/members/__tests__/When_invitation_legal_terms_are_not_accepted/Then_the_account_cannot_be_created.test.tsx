import { fireEvent, screen } from "@testing-library/react-native";
import { acceptInvitation, checkInvitation } from "@/features/authentication/authenticationApi";
import { AcceptInvitationScreen } from "@/features/members/screens/AcceptInvitationScreen";
import { translate } from "@/i18n/translate";
import { buildCheckedInvitation, buildTokenResponse } from "@/testing/authenticationFactory";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const password = "a long passphrase";

describe("When invitation legal terms are not accepted", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: "invitation-token-from-the-link" };
    jest.mocked(checkInvitation).mockResolvedValue(buildCheckedInvitation());
    jest.mocked(acceptInvitation).mockResolvedValue(buildTokenResponse());
  });

  it("Then the account cannot be created", async () => {
    await renderWithSession(<AcceptInvitationScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("invitation.fullName")),
      "Marcos Díaz",
    );
    await fireEvent.changeText(screen.getByLabelText(translate("birthDate.label")), "12041990");
    await fireEvent.changeText(screen.getByLabelText(translate("invitation.password")), password);
    await fireEvent.changeText(
      screen.getByLabelText(translate("invitationAnswer.passwordConfirmation")),
      password,
    );

    const submit = screen.getByRole("button", { name: translate("invitation.submit") });
    await fireEvent.press(submit);

    expect(submit).toBeDisabled();
    expect(acceptInvitation).not.toHaveBeenCalled();
  });
});
