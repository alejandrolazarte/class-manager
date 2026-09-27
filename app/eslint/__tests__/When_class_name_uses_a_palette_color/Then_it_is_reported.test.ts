import { lintWithThemeRules } from "../lintWithThemeRules";

describe("When class name uses a palette color", () => {
  it("Then it is reported", () => {
    const messages = lintWithThemeRules(
      'const row = <View className={`border-b ${isActive ? "bg-gray-50" : "bg-white"}`} />;',
    );

    expect(messages.map((message) => message.ruleId)).toEqual([
      "theme/no-raw-colors",
      "theme/no-raw-colors",
    ]);
  });
});
