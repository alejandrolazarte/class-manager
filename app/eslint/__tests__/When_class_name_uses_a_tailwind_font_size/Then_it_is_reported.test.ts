import { lintWithThemeRules } from "../lintWithThemeRules";

describe("When class name uses a tailwind font size", () => {
  it("Then it is reported", () => {
    const messages = lintWithThemeRules(
      'const name = <AppText variant="bodyStrong" className={`${spacing} text-base`} />;',
    );

    expect(messages.map((message) => message.ruleId)).toEqual(["theme/no-raw-font-sizes"]);
  });
});
