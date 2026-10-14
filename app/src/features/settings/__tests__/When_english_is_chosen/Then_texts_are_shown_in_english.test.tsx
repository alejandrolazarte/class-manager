import { fireEvent, screen } from "@testing-library/react-native";
import { LanguageSettings } from "@/features/settings/components/LanguageSettings";
import englishTranslations from "@/i18n/en.json";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { AppText } from "@/ui/AppText";

function SettingsTitleProbe() {
  return <AppText>{translate("settings.title")}</AppText>;
}

describe("When english is chosen", () => {
  it("Then texts are shown in english", async () => {
    await renderWithProviders(
      <>
        <LanguageSettings />
        <SettingsTitleProbe />
      </>,
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("settings.language.en") }));

    expect(screen.getByText(englishTranslations["settings.title"])).toBeOnTheScreen();
  });
});
