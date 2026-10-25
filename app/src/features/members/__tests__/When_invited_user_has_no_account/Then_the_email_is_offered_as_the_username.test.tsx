import { screen } from "@testing-library/react-native";
import { checkInvitation } from "@/features/authentication/authenticationApi";
import { AcceptInvitationScreen } from "@/features/members/screens/AcceptInvitationScreen";
import { translate } from "@/i18n/translate";
import { buildCheckedInvitation } from "@/testing/authenticationFactory";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const invitedEmail = "marcos@example.com";

describe("When invited user has no account", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: "invitation-token-from-the-link" };
    jest
      .mocked(checkInvitation)
      .mockResolvedValue(buildCheckedInvitation({ email: invitedEmail, hasAccount: false }));
  });

  it("Then the email is offered as the username", async () => {
    await renderWithSession(<AcceptInvitationScreen />);
    await screen.findByLabelText(translate("invitation.fullName"));

    expect(
      screen.getByDisplayValue(invitedEmail, { includeHiddenElements: true }).props.autoComplete,
    ).toBe("username");
  });
});
