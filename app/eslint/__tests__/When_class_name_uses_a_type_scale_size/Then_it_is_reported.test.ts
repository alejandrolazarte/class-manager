import { lintWithThemeRules } from "../lintWithThemeRules";

describe("When class name uses a type scale size", () => {
  it("Then it is reported", () => {
    const messages = lintWithThemeRules('const name = <AppText className="text-large" />;');

    expect(messages.map((message) => message.ruleId)).toEqual(["theme/no-raw-font-sizes"]);
  });
});
