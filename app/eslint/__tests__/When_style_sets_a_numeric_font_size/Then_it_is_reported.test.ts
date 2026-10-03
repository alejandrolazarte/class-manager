import { lintWithThemeRules } from "../lintWithThemeRules";

describe("When style sets a numeric font size", () => {
  it("Then it is reported", () => {
    const messages = lintWithThemeRules("const labelStyle = { fontFamily, fontSize: 12 };");

    expect(messages.map((message) => message.ruleId)).toEqual(["theme/no-raw-font-sizes"]);
  });
});
