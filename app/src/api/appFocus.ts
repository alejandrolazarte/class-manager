import { focusManager } from "@tanstack/react-query";
import { AppState, Platform } from "react-native";

const activeAppState = "active";

export function subscribeToAppFocus(onFocusChange: (isFocused: boolean) => void): () => void {
  const subscription = AppState.addEventListener("change", (status) =>
    onFocusChange(status === activeAppState),
  );
  return () => subscription.remove();
}

export function refetchWhenAppReturns(): void {
  if (Platform.OS !== "web") {
    focusManager.setEventListener(subscribeToAppFocus);
  }
}
