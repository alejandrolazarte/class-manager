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

const typeScale = {
  micro: { fontSize: 11, lineHeight: 14 },
  small: { fontSize: 12, lineHeight: 16 },
  caption: { fontSize: 13, lineHeight: 17 },
  body: { fontSize: 15, lineHeight: 21 },
  large: { fontSize: 17, lineHeight: 22 },
  title: { fontSize: 20, lineHeight: 24 },
  display: { fontSize: 26, lineHeight: 30 },
  hero: { fontSize: 32, lineHeight: 36 },
};

const themeColors = Object.fromEntries(
  themeColorTokens.map((token) => [token, `rgb(var(--color-${token}) / <alpha-value>)`]),
);

module.exports = {
  content: ["./app/**/*.{ts,tsx}", "./src/**/*.{ts,tsx}"],
  presets: [require("nativewind/preset")],
  darkMode: "class",
  theme: {
    fontSize: Object.fromEntries(
      Object.entries(typeScale).map(([size, { fontSize, lineHeight }]) => [
        size,
        [`${fontSize}px`, `${lineHeight}px`],
      ]),
    ),
    extend: {
      colors: themeColors,
      fontFamily: Object.fromEntries(
        Object.entries(fontFamilies).map(([role, family]) => [role, [family]]),
      ),
    },
  },
  plugins: [],
};
