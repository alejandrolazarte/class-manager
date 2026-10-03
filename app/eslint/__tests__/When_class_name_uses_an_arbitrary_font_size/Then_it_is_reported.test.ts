import { lintWithThemeRules } from "../lintWithThemeRules";

describe("When class name uses an arbitrary font size", () => {
  it("Then it is reported", () => {
    const messages = lintWithThemeRules(
      'const label = <AppText variant="badge" className="text-[10px] leading-3" />;',
    );

    expect(messages.map((message) => message.ruleId)).toEqual(["theme/no-raw-font-sizes"]);
  });
});
