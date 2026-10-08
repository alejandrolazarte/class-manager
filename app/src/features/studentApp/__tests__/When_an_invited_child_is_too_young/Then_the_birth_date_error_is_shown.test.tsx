import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import {
  acceptStudentAppInvitation,
  checkStudentAppInvitation,
} from "@/features/authentication/authenticationApi";
import { authenticationErrorCodes } from "@/features/authentication/authenticationErrorCodes";
import { AcceptStudentAppInvitationScreen } from "@/features/studentApp/screens/AcceptStudentAppInvitationScreen";
import { translate } from "@/i18n/translate";
import { buildCheckedInvitation } from "@/testing/authenticationFactory";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const badRequestStatus = 400;
const password = "a long passphrase";

describe("When an invited child is too young", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: "invitation-token" };
    jest
      .mocked(checkStudentAppInvitation)
      .mockResolvedValue(
        buildCheckedInvitation({ fullName: "Tomás Pérez", birthDate: "2012-05-01" }),
      );
    jest
      .mocked(acceptStudentAppInvitation)
      .mockRejectedValue(
        new ApiError(badRequestStatus, { code: authenticationErrorCodes.tooYoungForOwnAccount }),
      );
  });

  it("Then the birth date error is shown", async () => {
    await renderWithSession(<AcceptStudentAppInvitationScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("birthDate.label")),
      "01052016",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("studentAppInvitation.password")),
      password,
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("invitationAnswer.passwordConfirmation")),
      password,
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("studentAppInvitation.submit") }),
    );

    expect(await screen.findByText(translate("invitationAnswer.tooYoung"))).toBeOnTheScreen();
  });
});
