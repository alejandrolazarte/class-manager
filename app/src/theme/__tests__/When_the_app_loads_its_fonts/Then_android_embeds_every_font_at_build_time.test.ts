import appConfiguration from "../../../../app.json";
import { fontAssets } from "@/theme/typography";

const fontPluginName = "expo-font";

interface EmbeddedFontFamily {
  fontFamily: string;
}

interface FontPluginOptions {
  android?: { fonts?: EmbeddedFontFamily[] };
}

function findEmbeddedAndroidFontFamilies(): string[] {
  const fontPlugin = appConfiguration.expo.plugins.find(
    (plugin) => Array.isArray(plugin) && plugin[0] === fontPluginName,
  );
  const fontPluginOptions = (Array.isArray(fontPlugin) ? fontPlugin[1] : {}) as FontPluginOptions;
  return (fontPluginOptions.android?.fonts ?? []).map((font) => font.fontFamily);
}

describe("When the app loads its fonts", () => {
  it("Then android embeds every font at build time", () => {
    expect(findEmbeddedAndroidFontFamilies().sort()).toEqual(Object.keys(fontAssets).sort());
  });
});
