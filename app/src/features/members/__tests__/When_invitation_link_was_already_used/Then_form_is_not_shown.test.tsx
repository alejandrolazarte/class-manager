import { screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { checkInvitation } from "@/features/authentication/authenticationApi";
import { memberErrorCodes } from "@/features/members/memberErrorCodes";
import { AcceptInvitationScreen } from "@/features/members/screens/AcceptInvitationScreen";
import { translate } from "@/i18n/translate";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const badRequestStatus = 400;

describe("When invitation link was already used", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: "already-used-token" };
    jest
      .mocked(checkInvitation)
      .mockRejectedValue(
        new ApiError(badRequestStatus, { code: memberErrorCodes.invalidInvitation }),
      );
  });

  it("Then form is not shown", async () => {
    await renderWithSession(<AcceptInvitationScreen />);

    expect(await screen.findByText(translate("invitation.invalid"))).toBeOnTheScreen();
    expect(screen.queryByLabelText(translate("invitation.password"))).not.toBeOnTheScreen();
  });
});
