import { screen } from "@testing-library/react-native";
import { getMyAccount } from "@/features/account/accountApi";
import { AccountLanguageSync } from "@/features/account/components/AccountLanguageSync";
import englishTranslations from "@/i18n/en.json";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { AppText } from "@/ui/AppText";

jest.mock("@/features/account/accountApi");

function SettingsTitleProbe() {
  return <AppText>{translate("settings.title")}</AppText>;
}

describe("When the account prefers english", () => {
  beforeEach(() => {
    jest.mocked(getMyAccount).mockResolvedValue({
      email: "tomas@example.com",
      fullName: "Tomás Pérez",
      hasAcceptedCurrentLegalTerms: true,
      preferredLanguage: "en",
    });
  });

  it("Then the app is shown in english", async () => {
    await renderWithProviders(
      <>
        <AccountLanguageSync />
        <SettingsTitleProbe />
      </>,
    );

    expect(await screen.findByText(englishTranslations["settings.title"])).toBeOnTheScreen();
  });
});
