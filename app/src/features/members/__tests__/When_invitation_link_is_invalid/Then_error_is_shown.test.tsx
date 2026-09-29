import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/apiErrors";
import { acceptInvitation } from "@/features/authentication/authenticationApi";
import { memberErrorCodes } from "@/features/members/memberErrorCodes";
import { AcceptInvitationScreen } from "@/features/members/screens/AcceptInvitationScreen";
import { translate } from "@/i18n/translate";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const badRequestStatus = 400;

describe("When invitation link is invalid", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: "already-used-token" };
    jest.mocked(acceptInvitation).mockRejectedValue(
      new ApiError(badRequestStatus, {
        status: badRequestStatus,
        code: memberErrorCodes.invalidInvitation,
      }),
    );
  });

  it("Then error is shown", async () => {
    await renderWithSession(<AcceptInvitationScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("invitation.submit") }),
    );

    expect(await screen.findByText(translate("invitation.invalid"))).toBeOnTheScreen();
  });
});
