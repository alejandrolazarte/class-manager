import { screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { checkStudentAppInvitation } from "@/features/authentication/authenticationApi";
import { studentAppErrorCodes } from "@/features/studentApp/studentAppErrorCodes";
import { AcceptStudentAppInvitationScreen } from "@/features/studentApp/screens/AcceptStudentAppInvitationScreen";
import { translate } from "@/i18n/translate";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const badRequestStatus = 400;

describe("When student invitation link is invalid", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: "used-token" };
    jest
      .mocked(checkStudentAppInvitation)
      .mockRejectedValue(
        new ApiError(badRequestStatus, { code: studentAppErrorCodes.invalidInvitation }),
      );
  });

  it("Then error is shown", async () => {
    await renderWithSession(<AcceptStudentAppInvitationScreen />);

    expect(await screen.findByText(translate("studentAppInvitation.invalid"))).toBeOnTheScreen();
  });
});
