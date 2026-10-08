import { fireEvent, screen } from "@testing-library/react-native";
import {
  acceptInvitation,
  checkInvitation,
  declineInvitation,
} from "@/features/authentication/authenticationApi";
import { AcceptInvitationScreen } from "@/features/members/screens/AcceptInvitationScreen";
import { translate } from "@/i18n/translate";
import { buildCheckedInvitation } from "@/testing/authenticationFactory";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const invitationToken = "invitation-token-from-the-link";

describe("When invitation is declined", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: invitationToken };
    jest.mocked(checkInvitation).mockResolvedValue(buildCheckedInvitation({ hasAccount: true }));
    jest.mocked(declineInvitation).mockResolvedValue();
  });

  it("Then nobody joins the team", async () => {
    await renderWithSession(<AcceptInvitationScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("invitationAnswer.decline") }),
    );

    expect(await screen.findByText(translate("invitationAnswer.declined"))).toBeOnTheScreen();
    expect(declineInvitation).toHaveBeenCalledWith({ token: invitationToken });
    expect(acceptInvitation).not.toHaveBeenCalled();
  });
});
