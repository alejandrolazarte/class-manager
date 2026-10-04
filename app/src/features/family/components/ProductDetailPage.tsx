import { useState } from "react";
import { View } from "react-native";
import { ProductPhotoCarousel } from "@/features/family/components/ProductPhotoCarousel";
import { maximumUnitsPerItem } from "@/features/family/familyCart";
import { QuantityStepper } from "@/features/family/components/QuantityStepper";
import { FamilyShopProduct } from "@/features/family/types";
import { formatMoney } from "@/features/fees/money";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { FullScreenPage } from "@/ui/FullScreenPage";

interface ProductDetailPageProps {
  product: FamilyShopProduct;
  currencyCode: string;
  onClose: () => void;
  onAdd: (variantId: string, units: number) => void;
}

export function ProductDetailPage({
  product,
  currencyCode,
  onClose,
  onAdd,
}: ProductDetailPageProps) {
  const [selectedVariantId, setSelectedVariantId] = useState(
    product.variants.find((variant) => variant.availability !== "SoldOut")?.id ?? null,
  );
  const [units, setUnits] = useState(1);
  const selectedVariant = product.variants.find((variant) => variant.id === selectedVariantId);
  const hasSizes = product.variants.length > 1 || (product.variants[0]?.name.length ?? 0) > 0;
  const note =
    selectedVariant === undefined
      ? translate("family.shop.soldOutNote")
      : selectedVariant.availability === "OnOrder"
        ? translate("family.shop.onOrderNote")
        : null;

  return (
    <FullScreenPage
      title={product.name}
      onClose={onClose}
      footer={
        <View className="flex-row items-center gap-2.5 border-t border-border bg-background px-5 py-3">
          <View className="h-[52px] justify-center rounded-2xl border-[1.5px] border-border bg-surface">
            <QuantityStepper
              units={units}
              minimum={1}
              maximum={maximumUnitsPerItem}
              onChange={setUnits}
            />
          </View>
          <View className="flex-1">
            <Button
              label={
                selectedVariant === undefined
                  ? translate("family.shop.soldOut")
                  : translate("family.shop.addToCart", {
                      total: formatMoney(product.price * units, currencyCode),
                    })
              }
              disabled={selectedVariant === undefined}
              onPress={() => selectedVariant && onAdd(selectedVariant.id, units)}
            />
          </View>
        </View>
      }
    >
      <ProductPhotoCarousel images={product.images} sizeClassName="aspect-square" size="page" />
      <View className="gap-3.5 px-5 pb-6 pt-[18px]">
        <View className="gap-1">
          <AppText variant="display">{product.name}</AppText>
          <AppText variant="headline" tone="primary">
            {formatMoney(product.price, currencyCode)}
          </AppText>
        </View>
        {product.description ? (
          <AppText variant="body" tone="muted">
            {product.description}
          </AppText>
        ) : null}
        {hasSizes ? (
          <View className="gap-2">
            <AppText variant="label" tone="muted">
              {translate("family.shop.size")}
            </AppText>
            <View className="flex-row flex-wrap gap-2">
              {product.variants.map((variant) => (
                <Chip
                  key={variant.id}
                  shape="tile"
                  className="min-w-[52px] px-3"
                  label={variant.name}
                  isSelected={variant.id === selectedVariantId}
                  disabled={variant.availability === "SoldOut"}
                  onPress={() => setSelectedVariantId(variant.id)}
                />
              ))}
            </View>
          </View>
        ) : null}
        {note === null ? null : (
          <View className="rounded-[14px] bg-warning-soft px-3 py-2.5">
            <AppText variant="label" tone="warningSoft">
              {note}
            </AppText>
          </View>
        )}
      </View>
    </FullScreenPage>
  );
}
