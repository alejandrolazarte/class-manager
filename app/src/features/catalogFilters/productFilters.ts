import { ListFilter, uniqueOptions } from "@/filters/listFilters";
import { productColorName, productColorSwatch } from "@/features/products/productColors";
import { translate } from "@/i18n/translate";

export const productFilterIds = {
  kind: "kind",
  availability: "availability",
  size: "size",
  color: "color",
  status: "status",
} as const;

export const availabilityValues = {
  available: "available",
  soldOut: "soldOut",
} as const;

export function availabilityFilter({
  offersSoldOut,
  hint,
}: {
  offersSoldOut: boolean;
  hint?: string;
}): ListFilter {
  return {
    id: productFilterIds.availability,
    title: translate("filters.availability"),
    choice: "single",
    allLabel: translate("filters.availability.all"),
    hint,
    options: [
      { value: availabilityValues.available, label: translate("filters.availability.available") },
      ...(offersSoldOut
        ? [{ value: availabilityValues.soldOut, label: translate("filters.availability.soldOut") }]
        : []),
    ],
  };
}

export function sizeFilter(sizes: readonly string[]): ListFilter[] {
  const options = uniqueOptions(sizes.map((size) => ({ value: size, label: size })));
  if (options.length === 0) {
    return [];
  }
  return [
    {
      id: productFilterIds.size,
      title: translate("filters.size"),
      choice: "multiple",
      showsChips: true,
      options,
    },
  ];
}

export function colorFilter(colors: readonly (string | null)[]): ListFilter[] {
  const options = uniqueOptions(
    colors
      .filter((color): color is string => color !== null)
      .map((color) => ({
        value: color,
        label: productColorName(color),
        swatch: productColorSwatch(color),
      })),
  );
  if (options.length === 0) {
    return [];
  }
  return [
    {
      id: productFilterIds.color,
      title: translate("filters.color"),
      choice: "multiple",
      options,
    },
  ];
}
