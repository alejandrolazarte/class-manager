const paletteColorNames = [
  "black",
  "white",
  "slate",
  "gray",
  "zinc",
  "neutral",
  "stone",
  "red",
  "orange",
  "amber",
  "yellow",
  "lime",
  "green",
  "emerald",
  "teal",
  "cyan",
  "sky",
  "blue",
  "indigo",
  "violet",
  "purple",
  "fuchsia",
  "pink",
  "rose",
];

const colorUtilityPrefixes = [
  "bg",
  "text",
  "border",
  "border-[trblxyse]",
  "divide",
  "ring",
  "ring-offset",
  "outline",
  "placeholder",
  "shadow",
  "decoration",
  "accent",
  "caret",
  "fill",
  "stroke",
  "from",
  "via",
  "to",
];

const paletteClassPattern = new RegExp(
  `^(${colorUtilityPrefixes.join("|")})-(${paletteColorNames.join("|")})(-\\d{2,3})?(\\/\\d+)?$`,
);
const literalColorPattern = /^(#([0-9a-f]{3,4}|[0-9a-f]{6}|[0-9a-f]{8})|(rgba?|hsla?)\(.*\))$/i;
const variantSeparator = ":";

function withoutVariants(className) {
  return className.split(variantSeparator).pop();
}

function findRawColor(text) {
  const trimmedText = text.trim();
  if (literalColorPattern.test(trimmedText)) {
    return trimmedText;
  }
  return trimmedText
    .split(/\s+/)
    .find((className) => paletteClassPattern.test(withoutVariants(className)));
}

const noRawColors = {
  meta: {
    type: "problem",
    docs: {
      description:
        "Colors come from the theme: use semantic token classes (bg-surface, text-primary) or useTheme().colors.",
    },
    messages: {
      rawColor:
        '"{{rawColor}}" bypasses the theme. Use a semantic token (see docs/frontend/theming.md).',
    },
    schema: [],
  },
  create(context) {
    function check(node, text) {
      const rawColor = typeof text === "string" ? findRawColor(text) : undefined;
      if (rawColor) {
        context.report({ node, messageId: "rawColor", data: { rawColor } });
      }
    }
    return {
      Literal: (node) => check(node, node.value),
      TemplateElement: (node) => check(node, node.value.cooked),
    };
  },
};

module.exports = {
  meta: { name: "theme" },
  rules: { "no-raw-colors": noRawColors },
};
