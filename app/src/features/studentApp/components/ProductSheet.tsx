import { useState } from "react";
import { View } from "react-native";
import { ProductPhotoCarousel } from "@/features/studentApp/components/ProductPhotoCarousel";
import { maximumUnitsPerItem } from "@/features/studentApp/studentAppCart";
import { QuantityStepper } from "@/features/studentApp/components/QuantityStepper";
import { StudentAppShopProduct } from "@/features/studentApp/types";
import { formatMoney } from "@/features/fees/money";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { BottomSheet } from "@/ui/BottomSheet";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { IconButton } from "@/ui/IconButton";

interface ProductSheetProps {
  product: StudentAppShopProduct;
  currencyCode: string;
  onClose: () => void;
  onAdd: (variantId: string, units: number) => void;
}

export function ProductSheet({ product, currencyCode, onClose, onAdd }: ProductSheetProps) {
  const [selectedVariantId, setSelectedVariantId] = useState(
    product.variants.find((variant) => variant.availability !== "SoldOut")?.id ?? null,
  );
  const [units, setUnits] = useState(1);
  const selectedVariant = product.variants.find((variant) => variant.id === selectedVariantId);
  const hasSizes = product.variants.length > 1 || (product.variants[0]?.name.length ?? 0) > 0;
  const note =
    selectedVariant === undefined
      ? translate("student.shop.soldOutNote")
      : selectedVariant.availability === "OnOrder"
        ? translate("student.shop.onOrderNote")
        : null;

  return (
    <BottomSheet
      onClose={onClose}
      header={
        <View>
          <ProductPhotoCarousel images={product.images} heightClassName="h-[220px]" />
          <View className="absolute right-3 top-3 rounded-full bg-surface">
            <IconButton
              icon="close"
              accessibilityLabel={translate("common.close")}
              onPress={onClose}
            />
          </View>
        </View>
      }
    >
      <View className="flex-row items-start justify-between gap-3">
        <View className="min-w-0 flex-1 gap-1">
          <AppText variant="headline">{product.name}</AppText>
          {product.description ? (
            <AppText variant="body" tone="muted">
              {product.description}
            </AppText>
          ) : null}
        </View>
        <AppText variant="headline" tone="primary">
          {formatMoney(product.price, currencyCode)}
        </AppText>
      </View>
      {hasSizes ? (
        <View className="gap-2">
          <AppText variant="label" tone="muted">
            {translate("student.shop.size")}
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
      <View className="flex-row items-center gap-2.5">
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
                ? translate("student.shop.soldOut")
                : translate("student.shop.addToCart", {
                    total: formatMoney(product.price * units, currencyCode),
                  })
            }
            disabled={selectedVariant === undefined}
            onPress={() => selectedVariant && onAdd(selectedVariant.id, units)}
          />
        </View>
      </View>
    </BottomSheet>
  );
}
