import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import {
  acceptStudentAppInvitation,
  checkStudentAppInvitation,
} from "@/features/authentication/authenticationApi";
import { clientErrorCodes } from "@/features/clients/clientErrorCodes";
import { AcceptStudentAppInvitationScreen } from "@/features/studentApp/screens/AcceptStudentAppInvitationScreen";
import { translate } from "@/i18n/translate";
import { buildCheckedInvitation } from "@/testing/authenticationFactory";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const badRequestStatus = 400;

describe("When an invited contact is a minor", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: "invitation-token" };
    jest
      .mocked(checkStudentAppInvitation)
      .mockResolvedValue(buildCheckedInvitation({ hasAccount: true }));
    jest
      .mocked(acceptStudentAppInvitation)
      .mockRejectedValue(
        new ApiError(badRequestStatus, { code: clientErrorCodes.contactMustBeAdult }),
      );
  });

  it("Then the adult contact message is shown", async () => {
    await renderWithSession(<AcceptStudentAppInvitationScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("invitationAnswer.accept") }),
    );

    expect(
      await screen.findByText(translate("studentAppInvitation.contactMustBeAdult")),
    ).toBeOnTheScreen();
  });
});
