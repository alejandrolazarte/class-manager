import Ionicons from "@expo/vector-icons/Ionicons";
import { fontAssets } from "@/theme/typography";

describe("When the app loads its fonts", () => {
  it("Then the icon font is included", () => {
    expect(Object.keys(fontAssets)).toContain(Ionicons.getFontFamily());
  });
});
