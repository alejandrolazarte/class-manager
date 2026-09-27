import { Linter } from "eslint";

const themeRulesPlugin = jest.requireActual("../themeRules.js");

export function lintWithThemeRules(sourceCode: string): Linter.LintMessage[] {
  const linter = new Linter({ configType: "flat" });
  return linter.verify(
    sourceCode,
    [
      {
        files: ["**/*.tsx"],
        languageOptions: { parserOptions: { ecmaFeatures: { jsx: true } } },
        plugins: { theme: themeRulesPlugin },
        rules: { "theme/no-raw-colors": "error" },
      },
    ],
    "screen.tsx",
  );
}
