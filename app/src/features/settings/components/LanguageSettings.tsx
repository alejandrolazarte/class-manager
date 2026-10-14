import { View } from "react-native";
import { languages } from "@/i18n/languages";
import { translate } from "@/i18n/translate";
import { useLanguage } from "@/i18n/useLanguage";
import { languageValue } from "@/features/settings/components/languageValue";
import { AppText } from "@/ui/AppText";
import { SegmentedControl } from "@/ui/SegmentedControl";

export function LanguageSettings() {
  const { language, setLanguage } = useLanguage();
  return (
    <View className="gap-3">
      <AppText variant="headline" accessibilityRole="header">
        {translate("settings.language")}
      </AppText>
      <SegmentedControl
        options={languages.map((optionLanguage) => ({
          value: optionLanguage,
          label: languageValue(optionLanguage),
        }))}
        selectedValue={language}
        onChange={setLanguage}
      />
      <AppText variant="caption" tone="muted">
        {translate("settings.language.hint")}
      </AppText>
    </View>
  );
}
