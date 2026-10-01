import { useSegments } from "expo-router";

export function useCurrentTab<TTab extends string>(tabs: readonly [TTab, ...TTab[]]): TTab {
  const segments: readonly string[] = useSegments();
  return tabs.find((tab) => segments.includes(tab)) ?? tabs[0];
}
