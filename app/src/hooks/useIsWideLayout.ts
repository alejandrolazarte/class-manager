import { useWindowDimensions } from "react-native";

export const wideLayoutMinimumWidth = 768;

export function useIsWideLayout(): boolean {
  const { width } = useWindowDimensions();
  return width >= wideLayoutMinimumWidth;
}
