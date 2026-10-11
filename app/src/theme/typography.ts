import { iconFontAssets } from "@/ui/iconFont";
import { Figtree_400Regular } from "@expo-google-fonts/figtree/400Regular";
import { Figtree_500Medium } from "@expo-google-fonts/figtree/500Medium";
import { Figtree_600SemiBold } from "@expo-google-fonts/figtree/600SemiBold";
import { Figtree_700Bold } from "@expo-google-fonts/figtree/700Bold";

export const fontFamilies = {
  text: "Figtree_400Regular",
  label: "Figtree_500Medium",
  strong: "Figtree_600SemiBold",
  heavy: "Figtree_700Bold",
} as const;

export const fontAssets = {
  ...iconFontAssets,
  [fontFamilies.text]: Figtree_400Regular,
  [fontFamilies.label]: Figtree_500Medium,
  [fontFamilies.strong]: Figtree_600SemiBold,
  [fontFamilies.heavy]: Figtree_700Bold,
};

export const typeScale = {
  micro: { fontSize: 11, lineHeight: 14 },
  small: { fontSize: 12, lineHeight: 16 },
  caption: { fontSize: 13, lineHeight: 17 },
  body: { fontSize: 15, lineHeight: 21 },
  large: { fontSize: 17, lineHeight: 22 },
  title: { fontSize: 20, lineHeight: 24 },
  display: { fontSize: 24, lineHeight: 30 },
  hero: { fontSize: 28, lineHeight: 34 },
} as const;
