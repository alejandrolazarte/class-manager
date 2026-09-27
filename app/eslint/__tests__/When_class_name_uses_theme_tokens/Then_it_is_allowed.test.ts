import { lintWithThemeRules } from "../lintWithThemeRules";

describe("When class name uses theme tokens", () => {
  it("Then it is allowed", () => {
    const messages = lintWithThemeRules(
      'const row = <View className="border-b border-border-subtle bg-surface text-primary-foreground bg-danger/40 text-base" />;',
    );

    expect(messages).toEqual([]);
  });
});
