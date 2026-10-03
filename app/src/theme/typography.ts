import { iconFontAssets } from "@/ui/iconFont";
import { Nunito_600SemiBold } from "@expo-google-fonts/nunito/600SemiBold";
import { Nunito_700Bold } from "@expo-google-fonts/nunito/700Bold";
import { Nunito_800ExtraBold } from "@expo-google-fonts/nunito/800ExtraBold";
import { Nunito_900Black } from "@expo-google-fonts/nunito/900Black";

export const fontFamilies = {
  text: "Nunito_600SemiBold",
  label: "Nunito_700Bold",
  strong: "Nunito_800ExtraBold",
  heavy: "Nunito_900Black",
} as const;

export const fontAssets = {
  ...iconFontAssets,
  [fontFamilies.text]: Nunito_600SemiBold,
  [fontFamilies.label]: Nunito_700Bold,
  [fontFamilies.strong]: Nunito_800ExtraBold,
  [fontFamilies.heavy]: Nunito_900Black,
};

export const typeScale = {
  micro: { fontSize: 11, lineHeight: 14 },
  small: { fontSize: 12, lineHeight: 16 },
  caption: { fontSize: 13, lineHeight: 17 },
  body: { fontSize: 15, lineHeight: 21 },
  large: { fontSize: 17, lineHeight: 22 },
  title: { fontSize: 20, lineHeight: 24 },
  display: { fontSize: 26, lineHeight: 30 },
  hero: { fontSize: 32, lineHeight: 36 },
} as const;
