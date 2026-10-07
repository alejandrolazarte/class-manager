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

describe("When invited user has no account", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: "invitation-token-from-the-link" };
    jest.mocked(checkInvitation).mockResolvedValue(buildCheckedInvitation({ hasAccount: false }));
  });

  it("Then there is no decline button", async () => {
    await renderWithSession(<AcceptInvitationScreen />);

    expect(await screen.findByLabelText(translate("invitation.fullName"))).toBeOnTheScreen();
    expect(
      screen.queryByRole("button", { name: translate("invitationAnswer.decline") }),
    ).not.toBeOnTheScreen();
  });
});
