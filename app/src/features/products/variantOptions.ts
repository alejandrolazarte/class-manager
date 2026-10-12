interface VariantOptions {
  size: string;
  color: string | null;
}

export function sizesOf(variants: readonly VariantOptions[]): string[] {
  return [...new Set(variants.map((variant) => variant.size).filter((size) => size.length > 0))];
}

export function colorsOf(variants: readonly VariantOptions[]): string[] {
  return [
    ...new Set(
      variants.map((variant) => variant.color).filter((color): color is string => color !== null),
    ),
  ];
}
