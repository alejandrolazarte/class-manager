import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  checkGuardianConsent,
  giveGuardianConsent,
} from "@/features/authentication/authenticationApi";
import { AuthorizeStudentAppScreen } from "@/features/studentApp/screens/AuthorizeStudentAppScreen";
import { translate } from "@/i18n/translate";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const guardianToken = "guardian-token";

describe("When the guardian authorizes", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    searchParametersMock.current = { token: guardianToken };
    jest.mocked(checkGuardianConsent).mockResolvedValue({
      businessName: "Natación Olas",
      studentFullName: "Tomás Pérez",
      studentEmail: "tomas@example.com",
      guardianEmail: "ana@example.com",
      minimumAge: 13,
    });
    jest.mocked(giveGuardianConsent).mockResolvedValue();
  });

  it("Then the child is invited", async () => {
    await renderWithSession(<AuthorizeStudentAppScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("guardianConsent.authorize") }),
    );

    await waitFor(() => expect(giveGuardianConsent).toHaveBeenCalledWith({ token: guardianToken }));
    expect(
      await screen.findByText(
        translate("guardianConsent.authorized", { email: "tomas@example.com" }),
      ),
    ).toBeOnTheScreen();
  });
});
