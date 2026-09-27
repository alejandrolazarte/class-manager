import { Pressable, View } from "react-native";
import { hasTranslation, translate, TranslationKey } from "@/i18n/translate";
import { ColorSchemePreference } from "@/theme/ThemeContext";
import { ThemeName } from "@/theme/themes";
import { useTheme } from "@/theme/useTheme";
import { AppText } from "@/ui/AppText";
import { SegmentedControl } from "@/ui/SegmentedControl";

const colorSchemeOptions: readonly { value: ColorSchemePreference; labelKey: TranslationKey }[] = [
  { value: "system", labelKey: "settings.appearance.system" },
  { value: "light", labelKey: "settings.appearance.light" },
  { value: "dark", labelKey: "settings.appearance.dark" },
];

const themeTranslationPrefix = "themes.";

function themeDisplayName(themeName: ThemeName): string {
  const translationKey = `${themeTranslationPrefix}${themeName}`;
  return hasTranslation(translationKey) ? translate(translationKey) : themeName;
}

function ThemeSwatch({ themeName }: { themeName: ThemeName }) {
  const { themeName: activeThemeName, previewColors, setThemeName } = useTheme();
  const swatchColors = previewColors(themeName);
  const isSelected = themeName === activeThemeName;
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={themeDisplayName(themeName)}
      accessibilityState={{ selected: isSelected }}
      onPress={() => setThemeName(themeName)}
      className={`flex-1 items-center gap-1.5 rounded-2xl border-[1.5px] p-2.5 ${isSelected ? "border-primary bg-primary-soft" : "border-border bg-surface"}`}
    >
      <View className="flex-row">
        <View
          style={{ backgroundColor: swatchColors.primary }}
          className="h-7 w-7 rounded-full border-2 border-surface"
        />
        <View
          style={{ backgroundColor: swatchColors["primary-soft"] }}
          className="-ml-2 h-7 w-7 rounded-full border-2 border-surface"
        />
      </View>
      <AppText variant="badge" tone={isSelected ? "primarySoft" : "muted"} numberOfLines={1}>
        {themeDisplayName(themeName)}
      </AppText>
    </Pressable>
  );
}

export function AppearanceSettings() {
  const { themeNames, colorSchemePreference, setColorSchemePreference } = useTheme();
  return (
    <View className="gap-3.5 px-4 py-3.5">
      <View className="gap-2">
        <AppText variant="bodyStrong">{translate("settings.appearance")}</AppText>
        <SegmentedControl
          options={colorSchemeOptions.map(({ value, labelKey }) => ({
            value,
            label: translate(labelKey),
          }))}
          selectedValue={colorSchemePreference}
          onChange={setColorSchemePreference}
        />
      </View>
      {themeNames.length > 1 ? (
        <View className="gap-2">
          <AppText variant="bodyStrong">{translate("settings.colors")}</AppText>
          <View className="flex-row gap-2">
            {themeNames.map((themeName) => (
              <ThemeSwatch key={themeName} themeName={themeName} />
            ))}
          </View>
        </View>
      ) : null}
    </View>
  );
}
