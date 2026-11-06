import { Linking, Platform } from "react-native";

export async function openOrderCheckout(url: string): Promise<void> {
  if (Platform.OS === "web") {
    globalThis.location.assign(url);
    return;
  }
  await Linking.openURL(url);
}
