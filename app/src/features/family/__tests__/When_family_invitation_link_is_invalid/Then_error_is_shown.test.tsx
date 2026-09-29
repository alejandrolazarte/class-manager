import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { acceptFamilyInvitation } from "@/features/authentication/authenticationApi";
import { familyErrorCodes } from "@/features/family/familyErrorCodes";
import { AcceptFamilyInvitationScreen } from "@/features/family/screens/AcceptFamilyInvitationScreen";
import { translate } from "@/i18n/translate";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const badRequestStatus = 400;

describe("When family invitation link is invalid", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: "used-token" };
    jest
      .mocked(acceptFamilyInvitation)
      .mockRejectedValue(
        new ApiError(badRequestStatus, { code: familyErrorCodes.invalidInvitation }),
      );
  });

  it("Then error is shown", async () => {
    await renderWithSession(<AcceptFamilyInvitationScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("familyInvitation.fullName")),
      "Ana Pérez",
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("familyInvitation.submit") }),
    );

    expect(await screen.findByText(translate("familyInvitation.invalid"))).toBeOnTheScreen();
  });
});
