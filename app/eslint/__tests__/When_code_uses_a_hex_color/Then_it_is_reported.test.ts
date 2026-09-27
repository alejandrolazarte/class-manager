import { lintWithThemeRules } from "../lintWithThemeRules";

describe("When code uses a hex color", () => {
  it("Then it is reported", () => {
    const messages = lintWithThemeRules('const spinner = <ActivityIndicator color="#7c3aed" />;');

    expect(messages.map((message) => message.ruleId)).toEqual(["theme/no-raw-colors"]);
  });
});
