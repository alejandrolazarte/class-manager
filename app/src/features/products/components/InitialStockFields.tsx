import { View } from "react-native";
import { productLimits, unnamedVariantStockKey } from "@/features/products/productSchema";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { HairlineSection } from "@/ui/HairlineSection";
import { NumberStepper } from "@/ui/NumberStepper";

interface InitialStockFieldsProps {
  variantNames: string[];
  initialStock: Record<string, string>;
  onChange: (initialStock: Record<string, string>) => void;
}

const decimalRadix = 10;

function unitsOf(initialStockText: string | undefined): number {
  const units = Number.parseInt(initialStockText ?? "", decimalRadix);
  return Number.isNaN(units) ? 0 : units;
}

export function InitialStockFields({
  variantNames,
  initialStock,
  onChange,
}: InitialStockFieldsProps) {
  const stockKeys = variantNames.length === 0 ? [unnamedVariantStockKey] : variantNames;
  const totalUnits = stockKeys.reduce(
    (total, stockKey) => total + unitsOf(initialStock[stockKey]),
    0,
  );
  return (
    <HairlineSection
      title={translate("products.stock.single")}
      hint={translate("products.stock.initialHint")}
      trailing={
        <AppText variant="caption" tone="subtle">
          {translateCount("products.stock.units", totalUnits)}
        </AppText>
      }
    >
      <View className="gap-3">
        {stockKeys.map((stockKey) => {
          const label =
            stockKey === unnamedVariantStockKey ? translate("products.stock.single") : stockKey;
          return (
            <NumberStepper
              key={stockKey}
              label={label}
              value={initialStock[stockKey] ?? "0"}
              onChange={(value) => onChange({ ...initialStock, [stockKey]: value })}
              minimum={0}
              maximum={productLimits.maximumInitialStock}
              decreaseLabel={translate("products.stock.decrease", { name: label })}
              increaseLabel={translate("products.stock.increase", { name: label })}
            />
          );
        })}
      </View>
    </HairlineSection>
  );
}
