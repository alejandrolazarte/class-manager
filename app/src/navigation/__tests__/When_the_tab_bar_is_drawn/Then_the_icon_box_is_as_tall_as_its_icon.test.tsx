import { renderHook } from "@testing-library/react-native";
import { ReactNode } from "react";
import { StyleSheet } from "react-native";
import { SafeAreaProvider } from "react-native-safe-area-context";
import { useTabBarScreenOptions } from "@/navigation/tabBar";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { iconSizes } from "@/ui/Icon";

const safeAreaMetrics = {
  frame: { x: 0, y: 0, width: 390, height: 844 },
  insets: { top: 0, right: 0, bottom: 0, left: 0 },
};

function Providers({ children }: { children: ReactNode }) {
  return (
    <SafeAreaProvider initialMetrics={safeAreaMetrics}>
      <ThemeProvider initialColorSchemePreference="light">{children}</ThemeProvider>
    </SafeAreaProvider>
  );
}

describe("When the tab bar is drawn", () => {
  it("Then the icon box is as tall as its icon", async () => {
    const { result } = await renderHook(() => useTabBarScreenOptions(), { wrapper: Providers });

    expect(StyleSheet.flatten(result.current.tabBarIconStyle)?.height).toBe(iconSizes.large);
  });
});
