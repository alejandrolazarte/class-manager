import { View } from "react-native";
import { translate, TranslationKey } from "@/i18n/translate";
import { ColorSchemePreference } from "@/theme/ThemeContext";
import { useTheme } from "@/theme/useTheme";
import { AppText } from "@/ui/AppText";
import { SegmentedControl } from "@/ui/SegmentedControl";

const colorSchemeOptions: readonly { value: ColorSchemePreference; labelKey: TranslationKey }[] = [
  { value: "system", labelKey: "settings.appearance.system" },
  { value: "light", labelKey: "settings.appearance.light" },
  { value: "dark", labelKey: "settings.appearance.dark" },
];

interface AppearanceSettingsProps {
  colorsHint: string;
}

export function AppearanceSettings({ colorsHint }: AppearanceSettingsProps) {
  const { colorSchemePreference, setColorSchemePreference } = useTheme();
  return (
    <View className="gap-2 px-4 py-3.5">
      <AppText variant="bodyStrong">{translate("settings.appearance")}</AppText>
      <SegmentedControl
        options={colorSchemeOptions.map(({ value, labelKey }) => ({
          value,
          label: translate(labelKey),
        }))}
        selectedValue={colorSchemePreference}
        onChange={setColorSchemePreference}
      />
      <AppText variant="caption" tone="muted">
        {colorsHint}
      </AppText>
    </View>
  );
}
