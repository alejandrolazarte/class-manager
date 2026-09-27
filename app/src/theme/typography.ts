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
  [fontFamilies.text]: Nunito_600SemiBold,
  [fontFamilies.label]: Nunito_700Bold,
  [fontFamilies.strong]: Nunito_800ExtraBold,
  [fontFamilies.heavy]: Nunito_900Black,
};
