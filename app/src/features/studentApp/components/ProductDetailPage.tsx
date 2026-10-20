import { useState } from "react";
import { View } from "react-native";
import { ProductPhotoCarousel } from "@/features/studentApp/components/ProductPhotoCarousel";
import { maximumUnitsPerItem } from "@/features/studentApp/studentAppCart";
import { QuantityStepper } from "@/features/studentApp/components/QuantityStepper";
import { StudentAppShopProduct } from "@/features/studentApp/types";
import { formatMoney } from "@/formatting/money";
import { productColorName, productColorSwatch } from "@/features/products/productColors";
import { colorsOf } from "@/features/products/variantOptions";
import { checkColorOn } from "@/theme/productColorSwatches";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { ColorSwatchOption } from "@/ui/ColorSwatch";
import { FullScreenPage } from "@/ui/FullScreenPage";
import { Notice } from "@/ui/Notice";
import { Collapsible } from "@/ui/motion/Collapsible";

interface ProductDetailPageProps {
  product: StudentAppShopProduct;
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
  const colors = colorsOf(product.variants);
  const [selectedColor, setSelectedColor] = useState(
    product.variants.find((variant) => variant.id === selectedVariantId)?.color ??
      colors[0] ??
      null,
  );
  const selectedVariant = product.variants.find((variant) => variant.id === selectedVariantId);
  const variantsOfColor = product.variants.filter((variant) => variant.color === selectedColor);
  const hasSizes = variantsOfColor.some((variant) => variant.size.length > 0);
  const pickColor = (color: string) => {
    setSelectedColor(color);
    const sameSize = product.variants.find(
      (variant) =>
        variant.color === color &&
        variant.size === selectedVariant?.size &&
        variant.availability !== "SoldOut",
    );
    const firstAvailable = product.variants.find(
      (variant) => variant.color === color && variant.availability !== "SoldOut",
    );
    setSelectedVariantId((sameSize ?? firstAvailable)?.id ?? null);
  };
  const note =
    selectedVariant === undefined
      ? translate("student.shop.soldOutNote")
      : selectedVariant.availability === "OnOrder"
        ? translate("student.shop.onOrderNote")
        : null;

  return (
    <FullScreenPage
      title={product.name}
      onClose={onClose}
      footer={
        <View className="flex-row items-center gap-2.5 border-t border-border-subtle bg-background px-5 pb-4 pt-3">
          <View className="h-12 justify-center rounded-xl border border-border bg-surface">
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
        {colors.length > 0 ? (
          <View className="gap-2">
            <AppText variant="label" tone="muted">
              {translate("student.shop.color")}
            </AppText>
            <View className="flex-row flex-wrap">
              {colors.map((color) => {
                const swatch = productColorSwatch(color);
                return swatch === null ? null : (
                  <ColorSwatchOption
                    key={color}
                    swatch={swatch}
                    label={productColorName(color)}
                    isSelected={color === selectedColor}
                    checkColor={checkColorOn(swatch)}
                    onPress={() => pickColor(color)}
                  />
                );
              })}
            </View>
          </View>
        ) : null}
        {hasSizes ? (
          <View className="gap-2">
            <AppText variant="label" tone="muted">
              {translate("student.shop.size")}
            </AppText>
            <View className="flex-row flex-wrap gap-2">
              {variantsOfColor.map((variant) => (
                <Chip
                  key={variant.id}
                  shape="tile"
                  className="min-w-[52px] px-3"
                  label={variant.size}
                  isSelected={variant.id === selectedVariantId}
                  disabled={variant.availability === "SoldOut"}
                  onPress={() => setSelectedVariantId(variant.id)}
                />
              ))}
            </View>
          </View>
        ) : null}
        <Collapsible isExpanded={note !== null}>
          {note === null ? null : <Notice tone="warning" message={note} />}
        </Collapsible>
      </View>
    </FullScreenPage>
  );
}
