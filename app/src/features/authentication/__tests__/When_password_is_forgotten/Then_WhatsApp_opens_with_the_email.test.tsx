import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { Linking } from "react-native";
import { SignInScreen } from "@/features/authentication/screens/SignInScreen";
import { translate } from "@/i18n/translate";
import { ownerEmail } from "@/testing/fillAuthenticationForms";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const supportWhatsAppNumber = "34600111222";

describe("When password is forgotten", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    process.env.EXPO_PUBLIC_SUPPORT_WHATSAPP_NUMBER = supportWhatsAppNumber;
    jest.spyOn(Linking, "openURL").mockResolvedValue(true);
  });

  afterEach(() => {
    delete process.env.EXPO_PUBLIC_SUPPORT_WHATSAPP_NUMBER;
  });

  it("Then WhatsApp opens with the email", async () => {
    await renderWithSession(<SignInScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("authentication.signIn.email")),
      ownerEmail,
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("authentication.signIn.forgotPassword") }),
    );

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("authentication.signIn.askForResetLink"),
      }),
    );

    const expectedMessage = translate("authentication.signIn.resetLinkMessage", {
      email: ownerEmail,
    });
    await waitFor(() =>
      expect(Linking.openURL).toHaveBeenCalledWith(
        `https://wa.me/${supportWhatsAppNumber}?text=${encodeURIComponent(expectedMessage)}`,
      ),
    );
  });
});
