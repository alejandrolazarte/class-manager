import { screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { checkFamilyInvitation } from "@/features/authentication/authenticationApi";
import { familyErrorCodes } from "@/features/family/familyErrorCodes";
import { AcceptFamilyInvitationScreen } from "@/features/family/screens/AcceptFamilyInvitationScreen";
import { translate } from "@/i18n/translate";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const badRequestStatus = 400;

describe("When family invitation link was already used", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: "used-token" };
    jest
      .mocked(checkFamilyInvitation)
      .mockRejectedValue(
        new ApiError(badRequestStatus, { code: familyErrorCodes.invalidInvitation }),
      );
  });

  it("Then form is not shown", async () => {
    await renderWithSession(<AcceptFamilyInvitationScreen />);

    expect(await screen.findByText(translate("familyInvitation.invalid"))).toBeOnTheScreen();
    expect(screen.queryByLabelText(translate("familyInvitation.password"))).not.toBeOnTheScreen();
  });
});
