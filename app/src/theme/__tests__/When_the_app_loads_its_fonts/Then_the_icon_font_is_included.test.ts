import MaterialIcons from "@expo/vector-icons/MaterialIcons";
import { fontAssets } from "@/theme/typography";

describe("When the app loads its fonts", () => {
  it("Then the icon font is included", () => {
    expect(Object.keys(fontAssets)).toContain(MaterialIcons.getFontFamily());
  });
});
