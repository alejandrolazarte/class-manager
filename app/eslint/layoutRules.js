const titleComponentName = "TitleWithPills";
const statusPillComponentName = "StatusPill";
const smallPillAttributeName = "isSmall";
const tagComponentNames = ["InactiveChip"];
const overlayClassName = "absolute";
const transparentParentTypes = [
  "JSXExpressionContainer",
  "JSXFragment",
  "ConditionalExpression",
  "LogicalExpression",
];

function elementName(openingElement) {
  return openingElement.name.type === "JSXIdentifier" ? openingElement.name.name : undefined;
}

function isSmallPill(openingElement) {
  const name = elementName(openingElement);
  if (tagComponentNames.includes(name)) {
    return true;
  }
  return (
    name === statusPillComponentName &&
    openingElement.attributes.some(
      (attribute) =>
        attribute.type === "JSXAttribute" &&
        attribute.name.name === smallPillAttributeName &&
        (attribute.value === null ||
          (attribute.value.type === "JSXExpressionContainer" &&
            attribute.value.expression.value !== false)),
    )
  );
}

function isOverlay(openingElement) {
  return openingElement.attributes.some(
    (attribute) =>
      attribute.type === "JSXAttribute" &&
      attribute.name.name === "className" &&
      attribute.value?.type === "Literal" &&
      attribute.value.value.split(/\s+/).includes(overlayClassName),
  );
}

function enclosingElement(node) {
  let parent = node.parent;
  while (parent && transparentParentTypes.includes(parent.type)) {
    parent = parent.parent;
  }
  return parent && parent.type === "JSXElement" ? parent : undefined;
}

const pillsInTitle = {
  meta: {
    type: "problem",
    docs: {
      description:
        "Small pills tag a title: place them inside TitleWithPills (or an absolute overlay) so they never squeeze the text column.",
    },
    messages: {
      pillOutsideTitle:
        "Place this pill inside TitleWithPills from @/ui/TitleWithPills (see docs/frontend/theming.md).",
    },
    schema: [],
  },
  create(context) {
    return {
      JSXElement(node) {
        if (!isSmallPill(node.openingElement)) {
          return;
        }
        const parentElement = enclosingElement(node);
        if (
          parentElement &&
          elementName(parentElement.openingElement) !== titleComponentName &&
          !isOverlay(parentElement.openingElement)
        ) {
          context.report({ node, messageId: "pillOutsideTitle" });
        }
      },
    };
  },
};

module.exports = {
  meta: { name: "layout" },
  rules: { "pills-in-title": pillsInTitle },
};
