import { Pressable, View } from "react-native";
import { translate } from "@/i18n/translate";
import { builtInThemeNames } from "@/theme/builtInThemeColors";
import { BuiltInThemeName, ColorScheme, themes } from "@/theme/themes";
import { AppText } from "@/ui/AppText";

const optionsPerRow = 3;

interface ThemeOptionGridProps {
  selectedThemeName: BuiltInThemeName | null;
  colorScheme: ColorScheme;
  onSelect: (themeName: BuiltInThemeName) => void;
}

function toRows(themeNames: readonly BuiltInThemeName[]): (BuiltInThemeName | null)[][] {
  const rows: (BuiltInThemeName | null)[][] = [];
  for (let rowStart = 0; rowStart < themeNames.length; rowStart += optionsPerRow) {
    const row: (BuiltInThemeName | null)[] = themeNames.slice(rowStart, rowStart + optionsPerRow);
    while (row.length < optionsPerRow) {
      row.push(null);
    }
    rows.push(row);
  }
  return rows;
}

function ThemeOption({
  themeName,
  isSelected,
  colorScheme,
  onSelect,
}: {
  themeName: BuiltInThemeName;
  isSelected: boolean;
  colorScheme: ColorScheme;
  onSelect: (themeName: BuiltInThemeName) => void;
}) {
  const optionColors = themes[themeName][colorScheme];
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={translate(`themes.${themeName}`)}
      accessibilityState={{ selected: isSelected }}
      onPress={() => onSelect(themeName)}
      className={`flex-1 items-center gap-1.5 rounded-2xl border-[1.5px] p-2.5 ${isSelected ? "border-primary bg-primary-soft" : "border-border bg-surface"}`}
    >
      <View className="flex-row">
        <View
          style={{ backgroundColor: optionColors.primary }}
          className="h-7 w-7 rounded-full border-2 border-surface"
        />
        <View
          style={{ backgroundColor: optionColors["primary-soft"] }}
          className="-ml-2 h-7 w-7 rounded-full border-2 border-surface"
        />
      </View>
      <AppText variant="badge" tone={isSelected ? "primarySoft" : "muted"} numberOfLines={1}>
        {translate(`themes.${themeName}`)}
      </AppText>
    </Pressable>
  );
}

export function ThemeOptionGrid({
  selectedThemeName,
  colorScheme,
  onSelect,
}: ThemeOptionGridProps) {
  return (
    <View className="gap-2">
      {toRows(builtInThemeNames).map((row) => (
        <View key={row.join()} className="flex-row gap-2">
          {row.map((themeName, columnIndex) =>
            themeName ? (
              <ThemeOption
                key={themeName}
                themeName={themeName}
                isSelected={themeName === selectedThemeName}
                colorScheme={colorScheme}
                onSelect={onSelect}
              />
            ) : (
              <View key={columnIndex} className="flex-1 border-[1.5px] border-transparent p-2.5" />
            ),
          )}
        </View>
      ))}
    </View>
  );
}
