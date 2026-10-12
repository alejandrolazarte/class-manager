import { View } from "react-native";
import { ProductColorRowLabel } from "@/features/products/components/ProductColorRowLabel";
import { productColorName, variantDisplayName } from "@/features/products/productColors";
import { combinationsOf, productLimits, stockKeyOf } from "@/features/products/productSchema";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { HairlineSection } from "@/ui/HairlineSection";
import { NumberStepper } from "@/ui/NumberStepper";
import { StockGrid } from "@/ui/StockGrid";

interface InitialStockFieldsProps {
  sizes: string[];
  colors: string[];
  initialStock: Record<string, string>;
  onChange: (initialStock: Record<string, string>) => void;
}

const decimalRadix = 10;

function unitsOf(initialStockText: string | undefined): number {
  const units = Number.parseInt(initialStockText ?? "", decimalRadix);
  return Number.isNaN(units) ? 0 : units;
}

export function InitialStockFields({
  sizes,
  colors,
  initialStock,
  onChange,
}: InitialStockFieldsProps) {
  const combinations = combinationsOf(sizes, colors);
  const totalUnits = combinations.reduce(
    (total, combination) =>
      total + unitsOf(initialStock[stockKeyOf(combination.size, combination.color)]),
    0,
  );
  const isGrid = sizes.length > 0 && colors.length > 0;
  const changeUnits = (stockKey: string, value: string) =>
    onChange({ ...initialStock, [stockKey]: value });
  return (
    <HairlineSection
      title={translate("products.stock.single")}
      hint={translate(isGrid ? "products.stock.gridHint" : "products.stock.initialHint")}
      trailing={
        <AppText variant="caption" tone="subtle">
          {translateCount("products.stock.units", totalUnits)}
        </AppText>
      }
    >
      {isGrid ? (
        <StockGrid
          columnLabels={sizes}
          rows={colors.map((color) => ({
            key: color,
            label: productColorName(color),
            leading: <ProductColorRowLabel color={color} />,
            cells: sizes.map((size) => ({
              key: stockKeyOf(size, color),
              value: unitsOf(initialStock[stockKeyOf(size, color)]),
              accessibilityLabel: variantDisplayName(size, color),
            })),
          }))}
          onChangeCell={changeUnits}
        />
      ) : (
        <View className="gap-3">
          {combinations.map((combination) => {
            const stockKey = stockKeyOf(combination.size, combination.color);
            const displayName = variantDisplayName(combination.size, combination.color);
            const label =
              displayName.length === 0 ? translate("products.stock.single") : displayName;
            return (
              <NumberStepper
                key={stockKey}
                label={label}
                value={initialStock[stockKey] ?? "0"}
                onChange={(value) => changeUnits(stockKey, value)}
                minimum={0}
                maximum={productLimits.maximumInitialStock}
                decreaseLabel={translate("products.stock.decrease", { name: label })}
                increaseLabel={translate("products.stock.increase", { name: label })}
              />
            );
          })}
        </View>
      )}
    </HairlineSection>
  );
}
