import { fireEvent, screen } from "@testing-library/react-native";
import { LegalTermsConsent } from "@/features/legal/components/LegalTermsConsent";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

describe("When the terms link in the consent is pressed", () => {
  it("Then the terms open without checking the box", async () => {
    const changeAccepted = jest.fn();
    await renderWithProviders(
      <LegalTermsConsent isAccepted={false} onAcceptedChange={changeAccepted} />,
    );

    await fireEvent.press(screen.getByRole("link", { name: translate("legal.consent.terms") }));

    expect(routerMock.push).toHaveBeenCalledWith(routes.legalDocument("terms"));
    expect(changeAccepted).not.toHaveBeenCalled();
  });
});
