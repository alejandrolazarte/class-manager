import { fireEvent, screen } from "@testing-library/react-native";
import { updateMyLanguage } from "@/features/account/accountApi";
import { LanguageSettings } from "@/features/settings/components/LanguageSettings";
import spanishTranslations from "@/i18n/es.json";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { AppText } from "@/ui/AppText";

jest.mock("@/features/account/accountApi");

function SettingsTitleProbe() {
  return <AppText>{translate("settings.title")}</AppText>;
}

describe("When saving the language fails", () => {
  beforeEach(() => {
    jest.mocked(updateMyLanguage).mockRejectedValue(new Error("offline"));
  });

  it("Then the language does not change", async () => {
    await renderWithProviders(
      <>
        <LanguageSettings />
        <SettingsTitleProbe />
      </>,
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("settings.language.en") }));

    expect(await screen.findByText(translate("common.unexpectedError"))).toBeOnTheScreen();
    expect(screen.getByText(spanishTranslations["settings.title"])).toBeOnTheScreen();
  });
});
