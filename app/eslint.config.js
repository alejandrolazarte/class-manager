const { defineConfig } = require("eslint/config");
const expoConfig = require("eslint-config-expo/flat");
const prettierConfig = require("eslint-config-prettier/flat");
const themeRules = require("./eslint/themeRules");

const appSourceFiles = ["src/**/*.{ts,tsx}", "app/**/*.{ts,tsx}", "backoffice/**/*.{ts,tsx}"];
const backofficeFiles = ["backoffice/**/*.{ts,tsx}", "src/backoffice/**/*.{ts,tsx}"];
const testFiles = ["**/__tests__/**", "src/testing/**"];

const themedComponentImports = [
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
];
const iconImports = {
  group: ["@expo/vector-icons", "@expo/vector-icons/*"],
  message: "Use Icon from @/ui/Icon so icons come from one set and follow the theme.",
};
const backofficeImports = {
  group: ["@/backoffice", "@/backoffice/*"],
  message:
    "Only the backoffice imports src/backoffice: each web build carries only its own screens.",
};
const customerAppImports = {
  group: ["@/features", "@/features/*", "@/navigation", "@/navigation/*"],
  message:
    "The backoffice doesn't import the customer app's features or navigation: each web build carries only its own screens.",
};

module.exports = defineConfig([
  expoConfig,
  prettierConfig,
  {
    ignores: ["dist/*", "dist-backoffice/*", ".expo/*", "expo-env.d.ts"],
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
    ignores: ["src/theme/**", "src/ui/AppText.tsx", ...testFiles],
    plugins: { theme: themeRules },
    rules: {
      "theme/no-raw-font-sizes": "error",
    },
  },
  {
    files: appSourceFiles,
    ignores: ["src/ui/**", ...backofficeFiles, ...testFiles],
    rules: {
      "no-restricted-imports": [
        "error",
        { paths: themedComponentImports, patterns: [iconImports, backofficeImports] },
      ],
    },
  },
  {
    files: ["src/ui/**/*.{ts,tsx}"],
    ignores: testFiles,
    rules: {
      "no-restricted-imports": ["error", { patterns: [backofficeImports] }],
    },
  },
  {
    files: backofficeFiles,
    ignores: testFiles,
    rules: {
      "no-restricted-imports": [
        "error",
        { paths: themedComponentImports, patterns: [iconImports, customerAppImports] },
      ],
    },
  },
]);
