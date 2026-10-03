import { lintWithThemeRules } from "../lintWithThemeRules";

describe("When style takes its font size from the type scale", () => {
  it("Then it is allowed", () => {
    const messages = lintWithThemeRules(
      "const labelStyle = { fontSize: typeScale.small.fontSize, lineHeight: typeScale.small.lineHeight };",
    );

    expect(messages).toEqual([]);
  });
});
