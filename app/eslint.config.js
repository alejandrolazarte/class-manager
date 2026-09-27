const { defineConfig } = require("eslint/config");
const expoConfig = require("eslint-config-expo/flat");
const prettierConfig = require("eslint-config-prettier/flat");
const themeRules = require("./eslint/themeRules");

const appSourceFiles = ["src/**/*.{ts,tsx}", "app/**/*.{ts,tsx}"];
const testFiles = ["**/__tests__/**", "src/testing/**"];

module.exports = defineConfig([
  expoConfig,
  prettierConfig,
  {
    ignores: ["dist/*", ".expo/*", "expo-env.d.ts"],
  },
  {
    rules: {
      "prefer-const": "error",
      "react-hooks/exhaustive-deps": "error",
    },
  },
  {
    files: ["**/*.ts", "**/*.tsx"],
    rules: {
      "@typescript-eslint/no-explicit-any": "error",
      "@typescript-eslint/no-unused-vars": "error",
    },
  },
  {
    files: appSourceFiles,
    ignores: ["src/theme/**", ...testFiles],
    plugins: { theme: themeRules },
    rules: {
      "theme/no-raw-colors": "error",
    },
  },
  {
    files: appSourceFiles,
    ignores: ["src/ui/**", ...testFiles],
    rules: {
      "no-restricted-imports": [
        "error",
        {
          paths: [
            {
              name: "react-native",
              importNames: ["Text"],
              message: "Use AppText from @/ui/AppText so text follows the theme.",
            },
            {
              name: "react-native",
              importNames: ["ActivityIndicator"],
              message: "Use Spinner from @/ui/Spinner so it follows the theme.",
            },
          ],
          patterns: [
            {
              group: ["@expo/vector-icons", "@expo/vector-icons/*"],
              message: "Use Icon from @/ui/Icon so icons come from one set and follow the theme.",
            },
          ],
        },
      ],
    },
  },
]);
