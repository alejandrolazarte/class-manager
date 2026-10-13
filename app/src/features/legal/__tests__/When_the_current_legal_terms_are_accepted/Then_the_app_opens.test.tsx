import { screen } from "@testing-library/react-native";
import { Text } from "react-native";
import { getMyAccount } from "@/features/account/accountApi";
import { LegalTermsGate } from "@/features/legal/components/LegalTermsGate";
import { translate } from "@/i18n/translate";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/account/accountApi");
jest.mock("@/features/authentication/sessionStorage");

const appContent = "Inicio de la app";

describe("When the current legal terms are accepted", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    jest.mocked(getMyAccount).mockResolvedValue({
      email: "laura@example.com",
      fullName: "Laura Gómez",
      birthDate: "1990-04-12",
      hasAcceptedCurrentLegalTerms: true,
    });
  });

  it("Then the app opens", async () => {
    await renderWithSession(
      <LegalTermsGate>
        <Text>{appContent}</Text>
      </LegalTermsGate>,
    );

    expect(await screen.findByText(appContent)).toBeOnTheScreen();
    expect(screen.queryByText(translate("legal.update.title"))).not.toBeOnTheScreen();
  });
});
