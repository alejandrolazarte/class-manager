import { fireEvent, screen } from "@testing-library/react-native";
import {
  acceptStudentAppInvitation,
  checkStudentAppInvitation,
} from "@/features/authentication/authenticationApi";
import { AcceptStudentAppInvitationScreen } from "@/features/studentApp/screens/AcceptStudentAppInvitationScreen";
import { translate } from "@/i18n/translate";
import { buildCheckedInvitation } from "@/testing/authenticationFactory";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

describe("When the passwords do not match", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: "invitation-token" };
    jest
      .mocked(checkStudentAppInvitation)
      .mockResolvedValue(buildCheckedInvitation({ fullName: "Tomás Pérez" }));
  });

  it("Then the account is not created", async () => {
    await renderWithSession(<AcceptStudentAppInvitationScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("studentAppInvitation.password")),
      "a long passphrase",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("invitationAnswer.passwordConfirmation")),
      "another passphrase",
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("studentAppInvitation.submit") }),
    );

    expect(
      await screen.findByText(translate("invitationAnswer.passwordsDoNotMatch")),
    ).toBeOnTheScreen();
    expect(acceptStudentAppInvitation).not.toHaveBeenCalled();
  });
});
