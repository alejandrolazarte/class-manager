import { Platform } from "react-native";
import { tabAccessibilityLabel } from "@/ui/TabBar";

describe("When a tab is read on iOS", () => {
  it("Then its accessibility label names its position", () => {
    Platform.OS = "ios";
    const navigation = { getState: () => ({ routes: [{ key: "today" }, { key: "classes" }] }) };

    expect(tabAccessibilityLabel("Clases", navigation, "classes")).toBe("Clases, tab, 2 of 2");
  });
});
