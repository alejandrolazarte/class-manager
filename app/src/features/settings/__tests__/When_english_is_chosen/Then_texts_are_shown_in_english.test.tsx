import { fireEvent, screen } from "@testing-library/react-native";
import { updateMyLanguage } from "@/features/account/accountApi";
import { LanguageSettings } from "@/features/settings/components/LanguageSettings";
import englishTranslations from "@/i18n/en.json";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { AppText } from "@/ui/AppText";

jest.mock("@/features/account/accountApi");

function SettingsTitleProbe() {
  return <AppText>{translate("settings.title")}</AppText>;
}

describe("When english is chosen", () => {
  beforeEach(() => {
    jest.mocked(updateMyLanguage).mockResolvedValue({
      email: "tomas@example.com",
      fullName: "Tomás Pérez",
      hasAcceptedCurrentLegalTerms: true,
      preferredLanguage: "en",
    });
  });

  it("Then texts are shown in english", async () => {
    await renderWithProviders(
      <>
        <LanguageSettings />
        <SettingsTitleProbe />
      </>,
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("settings.language.en") }));

    expect(await screen.findByText(englishTranslations["settings.title"])).toBeOnTheScreen();
    expect(jest.mocked(updateMyLanguage).mock.calls[0]?.[0]).toBe("en");
  });
});
