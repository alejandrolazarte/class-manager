import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { acceptStudentAppInvitation } from "@/features/authentication/authenticationApi";
import { AcceptStudentAppInvitationScreen } from "@/features/studentApp/screens/AcceptStudentAppInvitationScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildTokenResponse } from "@/testing/authenticationFactory";
import { routerMock, searchParametersMock } from "@/testing/expoRouterMock";
import { buildStudentAccessToken } from "@/testing/studentAppFactory";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const invitationToken = "student-invitation-token";
const fullName = "Ana Pérez";
const password = "a long student passphrase";

describe("When student invitation is accepted", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: invitationToken };
    jest
      .mocked(acceptStudentAppInvitation)
      .mockResolvedValue(buildTokenResponse({ accessToken: buildStudentAccessToken() }));
  });

  it("Then student home opens", async () => {
    await renderWithSession(<AcceptStudentAppInvitationScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("studentAppInvitation.fullName")),
      fullName,
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("studentAppInvitation.password")),
      password,
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("studentAppInvitation.submit") }),
    );

    await waitFor(() => expect(routerMock.replace).toHaveBeenCalledWith(routes.studentApp));
    expect(acceptStudentAppInvitation).toHaveBeenCalledWith({
      token: invitationToken,
      fullName,
      password,
    });
  });
});
