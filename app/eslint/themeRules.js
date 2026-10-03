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

const typeScaleSizeNames = Object.keys(require("../tailwind.config.js").theme.fontSize);
const fontSizeClassPattern = new RegExp(
  `^(text-(xs|sm|base|lg|xl|[2-9]xl|\\[\\d[^\\]]*\\]|${typeScaleSizeNames.join("|")})|leading-.+)$`,
);
const fontSizeStyleProperties = ["fontSize", "lineHeight"];

function findRawFontSize(text) {
  return text
    .trim()
    .split(/\s+/)
    .find((className) => fontSizeClassPattern.test(withoutVariants(className)));
}

function propertyName(property) {
  return property.key.type === "Identifier" ? property.key.name : property.key.value;
}

const noRawFontSizes = {
  meta: {
    type: "problem",
    docs: {
      description: "Text sizes come from the type scale: use an AppText variant.",
    },
    messages: {
      rawFontSize:
        '"{{rawFontSize}}" bypasses the type scale. Use an AppText variant (see docs/frontend/theming.md).',
    },
    schema: [],
  },
  create(context) {
    function check(node, text) {
      const rawFontSize = typeof text === "string" ? findRawFontSize(text) : undefined;
      if (rawFontSize) {
        context.report({ node, messageId: "rawFontSize", data: { rawFontSize } });
      }
    }
    return {
      Literal: (node) => check(node, node.value),
      TemplateElement: (node) => check(node, node.value.cooked),
      Property: (node) => {
        if (
          fontSizeStyleProperties.includes(propertyName(node)) &&
          node.value.type === "Literal" &&
          typeof node.value.value === "number"
        ) {
          context.report({
            node,
            messageId: "rawFontSize",
            data: { rawFontSize: `${propertyName(node)}: ${node.value.value}` },
          });
        }
      },
    };
  },
};

module.exports = {
  meta: { name: "theme" },
  rules: { "no-raw-colors": noRawColors, "no-raw-font-sizes": noRawFontSizes },
};
