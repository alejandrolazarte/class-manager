const themeColorTokens = [
  "background",
  "foreground",
  "surface",
  "muted",
  "muted-foreground",
  "subtle-foreground",
  "disabled-foreground",
  "border",
  "border-subtle",
  "border-strong",
  "primary",
  "primary-foreground",
  "primary-strong",
  "primary-soft",
  "primary-soft-foreground",
  "accent",
  "accent-soft",
  "accent-soft-foreground",
  "danger",
  "danger-foreground",
  "danger-soft",
  "danger-soft-foreground",
  "warning",
  "warning-soft",
  "warning-soft-foreground",
  "success",
  "success-foreground",
  "success-soft",
  "success-soft-foreground",
  "inverse",
  "inverse-foreground",
  "shadow",
];

const fontFamilies = {
  text: "Nunito_600SemiBold",
  label: "Nunito_700Bold",
  strong: "Nunito_800ExtraBold",
  heavy: "Nunito_900Black",
};

const themeColors = Object.fromEntries(
  themeColorTokens.map((token) => [token, `rgb(var(--color-${token}) / <alpha-value>)`]),
);

module.exports = {
  content: ["./app/**/*.{ts,tsx}", "./src/**/*.{ts,tsx}"],
  presets: [require("nativewind/preset")],
  darkMode: "class",
  theme: {
    extend: {
      colors: themeColors,
      fontFamily: Object.fromEntries(
        Object.entries(fontFamilies).map(([role, family]) => [role, [family]]),
      ),
    },
  },
  plugins: [],
};
