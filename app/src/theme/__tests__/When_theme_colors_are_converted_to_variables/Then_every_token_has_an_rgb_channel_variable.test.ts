import { buildThemeVariables } from "@/theme/buildThemeVariables";
import { themeColorTokens } from "@/theme/themeColorTokens";
import { themes } from "@/theme/themes";

describe("When theme colors are converted to variables", () => {
  it("Then every token has an rgb channel variable", () => {
    const variables = buildThemeVariables(themes.violet.light);

    expect(Object.keys(variables)).toEqual(themeColorTokens.map((token) => `--color-${token}`));
    expect(variables["--color-primary"]).toBe("124 58 237");
  });
});
