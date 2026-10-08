import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  acceptStudentAppInvitation,
  checkStudentAppInvitation,
} from "@/features/authentication/authenticationApi";
import { AcceptStudentAppInvitationScreen } from "@/features/studentApp/screens/AcceptStudentAppInvitationScreen";
import { translate } from "@/i18n/translate";
import { buildCheckedInvitation, buildTokenResponse } from "@/testing/authenticationFactory";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const invitationToken = "invitation-token";
const password = "a long passphrase";

describe("When an invited child creates the account", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: invitationToken };
    jest
      .mocked(checkStudentAppInvitation)
      .mockResolvedValue(
        buildCheckedInvitation({ fullName: "Tomás Pérez", birthDate: "2012-05-01" }),
      );
    jest.mocked(acceptStudentAppInvitation).mockResolvedValue(buildTokenResponse());
  });

  it("Then only a password is asked", async () => {
    await renderWithSession(<AcceptStudentAppInvitationScreen />);
    expect(
      await screen.findByText(translate("invitationAnswer.greeting", { name: "Tomás Pérez" })),
    ).toBeOnTheScreen();
    expect(
      screen.queryByLabelText(translate("studentAppInvitation.fullName")),
    ).not.toBeOnTheScreen();
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

    await waitFor(() =>
      expect(acceptStudentAppInvitation).toHaveBeenCalledWith({
        token: invitationToken,
        fullName: "Tomás Pérez",
        password,
        birthDate: "2012-05-01",
      }),
    );
  });
});
