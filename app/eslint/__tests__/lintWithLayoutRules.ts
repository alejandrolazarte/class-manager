import { Linter } from "eslint";

const layoutRulesPlugin = jest.requireActual("../layoutRules.js");

export function lintWithLayoutRules(sourceCode: string): Linter.LintMessage[] {
  const linter = new Linter({ configType: "flat" });
  return linter.verify(
    sourceCode,
    [
      {
        files: ["**/*.tsx"],
        languageOptions: { parserOptions: { ecmaFeatures: { jsx: true } } },
        plugins: { layout: layoutRulesPlugin },
        rules: { "layout/pills-in-title": "error" },
      },
    ],
    "screen.tsx",
  );
}
