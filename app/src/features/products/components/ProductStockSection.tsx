import { useState } from "react";
import { View } from "react-native";
import { StockMovementSheet } from "@/features/products/components/StockMovementSheet";
import { variantLabel } from "@/features/products/stockLabels";
import { Product, StockLoadKind } from "@/features/products/types";
import { useStockMovements } from "@/features/products/useProducts";
import { translate, translateCount, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { HairlineSection } from "@/ui/HairlineSection";

interface ProductStockSectionProps {
  product: Product;
}

const recentMovementCount = 10;

export function ProductStockSection({ product }: ProductStockSectionProps) {
  const { data: movements = [] } = useStockMovements(product.id);
  const [sheetKind, setSheetKind] = useState<StockLoadKind | null>(null);
  const variantsById = new Map(product.variants.map((variant) => [variant.id, variant]));
  const totalUnits = product.variants.reduce((total, variant) => total + (variant.stock ?? 0), 0);
  const emptyStockKey: TranslationKey =
    product.stockMode === "TrackedWithBackorder"
      ? "products.stock.onOrder"
      : "products.stock.soldOut";
  return (
    <HairlineSection
      title={translate("products.stock.single")}
      trailing={
        <AppText variant="caption" tone="subtle">
          {translateCount("products.stock.units", totalUnits)}
        </AppText>
      }
    >
      {product.variants.map((variant) => {
        const units = variant.stock ?? 0;
        return (
          <View key={variant.id} className="flex-row justify-between">
            <AppText variant="body">{variantLabel(variant)}</AppText>
            {units > 0 ? (
              <AppText variant="bodyStrong">
                {translateCount("products.stock.units", units)}
              </AppText>
            ) : (
              <AppText variant="bodyStrong" tone="danger">
                {translate(emptyStockKey)}
              </AppText>
            )}
          </View>
        );
      })}
      <View className="flex-row gap-2">
        <View className="flex-1">
          <Button
            variant="outline"
            size="medium"
            label={translate("products.stock.load")}
            onPress={() => setSheetKind("Restock")}
          />
        </View>
        <View className="flex-1">
          <Button
            variant="secondary"
            size="medium"
            label={translate("products.stock.adjust")}
            onPress={() => setSheetKind("Adjustment")}
          />
        </View>
      </View>
      {movements.length > 0 ? (
        <View className="gap-1.5">
          <AppText variant="label" tone="muted">
            {translate("products.stock.history")}
          </AppText>
          {movements.slice(0, recentMovementCount).map((movement) => {
            const variant = variantsById.get(movement.variantId);
            return (
              <AppText key={movement.id} variant="caption" tone="subtle">
                {[
                  translate(`products.stock.kind.${movement.kind}` as TranslationKey),
                  `${movement.quantity > 0 ? "+" : ""}${movement.quantity}`,
                  variant && variant.name.length > 0 ? variant.name : null,
                  movement.note,
                ]
                  .filter(Boolean)
                  .join(" · ")}
              </AppText>
            );
          })}
        </View>
      ) : null}
      {sheetKind === null ? null : (
        <StockMovementSheet
          product={product}
          initialKind={sheetKind}
          onClose={() => setSheetKind(null)}
        />
      )}
    </HairlineSection>
  );
}
