import { fireEvent, screen } from "@testing-library/react-native";
import { Text } from "react-native";
import { acceptLegalTerms, getMyAccount } from "@/features/account/accountApi";
import { LegalTermsGate } from "@/features/legal/components/LegalTermsGate";
import { translate } from "@/i18n/translate";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/account/accountApi");
jest.mock("@/features/authentication/sessionStorage");

const appContent = "Inicio de la app";

describe("When the legal terms were updated", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    jest.mocked(getMyAccount).mockResolvedValue({
      email: "laura@example.com",
      fullName: "Laura Gómez",
      birthDate: "1990-04-12",
      hasAcceptedCurrentLegalTerms: false,
      preferredLanguage: "es",
    });
    jest.mocked(acceptLegalTerms).mockResolvedValue(undefined);
  });

  it("Then they are accepted before the app opens", async () => {
    await renderWithSession(
      <LegalTermsGate>
        <Text>{appContent}</Text>
      </LegalTermsGate>,
    );
    expect(await screen.findByText(translate("legal.update.title"))).toBeOnTheScreen();
    expect(screen.queryByText(appContent)).not.toBeOnTheScreen();

    await fireEvent.press(screen.getByRole("checkbox", { name: translate("legal.consent.label") }));
    await fireEvent.press(screen.getByRole("button", { name: translate("legal.update.submit") }));

    expect(await screen.findByText(appContent)).toBeOnTheScreen();
    expect(acceptLegalTerms).toHaveBeenCalledTimes(1);
  });
});
