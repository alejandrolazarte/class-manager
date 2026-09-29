import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import {
  cartLines,
  cartTotal,
  FamilyCart,
  maximumUnitsPerItem,
} from "@/features/family/familyCart";
import { familyErrorCodes } from "@/features/family/familyErrorCodes";
import { DeliveryPicker } from "@/features/family/components/DeliveryPicker";
import { FamilyShopProduct, FamilyShopVariant } from "@/features/family/types";
import { useFamilyShop, usePlaceFamilyOrder } from "@/features/family/useFamilyShop";
import { formatMoney } from "@/features/fees/money";
import { translate, translateCount, TranslationKey } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { NumberStepper } from "@/ui/NumberStepper";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { Spinner } from "@/ui/Spinner";
import { useToast } from "@/ui/ToastProvider";

type OrderProblem = "outOfStock" | "tooManyOpenOrders" | "unexpected";

const orderProblemMessages = {
  outOfStock: "family.shop.outOfStock",
  tooManyOpenOrders: "family.shop.tooManyOpenOrders",
  unexpected: "common.unexpectedError",
} as const;

const decimalRadix = 10;

function variantName(product: FamilyShopProduct, variant: FamilyShopVariant): string {
  return variant.name.length === 0 ? product.name : `${product.name} · ${variant.name}`;
}

export function FamilyShopScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const { data: shop, isPending, isError, refetch } = useFamilyShop();
  const placeFamilyOrderMutation = usePlaceFamilyOrder();
  const [cart, setCart] = useState<FamilyCart>({});
  const [deliveryClassGroupId, setDeliveryClassGroupId] = useState<string | null>(null);
  const [orderProblem, setOrderProblem] = useState<OrderProblem | null>(null);

  const setUnits = (itemId: string, units: number) => setCart({ ...cart, [itemId]: units });

  const placeOrder = async () => {
    if (shop === undefined) {
      return;
    }
    setOrderProblem(null);
    try {
      const lines = cartLines(shop, cart);
      const hasProducts = lines.some((line) => line.productVariantId !== null);
      await placeFamilyOrderMutation.mutateAsync({
        lines,
        delivery: hasProducts
          ? {
              delivery: deliveryClassGroupId === null ? "Pickup" : "InClass",
              deliveryClassGroupId,
            }
          : null,
      });
      setCart({});
      showToast(translate("family.shop.ordered"));
      router.replace(routes.familyOrders);
    } catch (orderError) {
      setOrderProblem(
        isApiError(orderError) && orderError.hasCode(familyErrorCodes.outOfStock)
          ? "outOfStock"
          : isApiError(orderError) && orderError.hasCode(familyErrorCodes.tooManyOpenOrders)
            ? "tooManyOpenOrders"
            : "unexpected",
      );
    }
  };

  const header = <ScreenHeader navigation="back" title={translate("family.shop.title")} />;
  if (isPending) {
    return (
      <ScrollScreen header={header}>
        <Spinner className="mt-6" />
      </ScrollScreen>
    );
  }
  if (isError || shop === undefined) {
    return (
      <ScrollScreen header={header}>
        <Banner message={translate("family.loadError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => refetch()}
          />
        </Banner>
      </ScrollScreen>
    );
  }

  const lines = cartLines(shop, cart);
  const total = cartTotal(shop, cart);
  return (
    <ScrollScreen header={header}>
      {shop.packs.length === 0 && shop.products.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate("family.shop.empty")}
        </AppText>
      ) : null}
      {shop.packs.length > 0 ? <SectionTitle title={translate("family.shop.packs")} /> : null}
      {shop.packs.map((pack) => {
        const isInCart = (cart[pack.id] ?? 0) > 0;
        return (
          <Card key={pack.id} className="gap-2 px-4 py-3.5">
            <AppText variant="bodyStrong">{pack.name}</AppText>
            <AppText variant="caption" tone="subtle">
              {[
                translateCount("family.shop.classCount", pack.classCount),
                pack.validityMonths === null
                  ? null
                  : translateCount("family.shop.validity", pack.validityMonths),
                formatMoney(pack.price, shop.currencyCode),
              ]
                .filter(Boolean)
                .join(" · ")}
            </AppText>
            <Button
              size="medium"
              variant={isInCart ? "outline" : "secondary"}
              label={translate(isInCart ? "family.shop.removePack" : "family.shop.addPack")}
              accessibilityLabel={`${translate(isInCart ? "family.shop.removePack" : "family.shop.addPack")} ${pack.name}`}
              onPress={() => setUnits(pack.id, isInCart ? 0 : 1)}
            />
          </Card>
        );
      })}
      {shop.products.length > 0 ? <SectionTitle title={translate("family.shop.products")} /> : null}
      {shop.products.map((product) => (
        <Card key={product.id} className="gap-2 px-4 py-3.5">
          <AppText variant="bodyStrong">
            {`${product.name} · ${formatMoney(product.price, shop.currencyCode)}`}
          </AppText>
          {product.description ? (
            <AppText variant="caption" tone="subtle">
              {product.description}
            </AppText>
          ) : null}
          {product.variants.map((variant) => {
            const name = variantName(product, variant);
            if (variant.availability === "SoldOut") {
              return (
                <AppText key={variant.id} variant="label" tone="muted">
                  {`${name} · ${translate("family.shop.soldOut")}`}
                </AppText>
              );
            }
            const label =
              variant.availability === "OnOrder"
                ? `${name} · ${translate("family.shop.onOrder")}`
                : name;
            return (
              <NumberStepper
                key={variant.id}
                label={label}
                value={String(cart[variant.id] ?? 0)}
                onChange={(unitsText) => {
                  const units = Number.parseInt(unitsText, decimalRadix);
                  setUnits(
                    variant.id,
                    Number.isNaN(units) ? 0 : Math.min(units, maximumUnitsPerItem),
                  );
                }}
                minimum={0}
                maximum={maximumUnitsPerItem}
                decreaseLabel={translate("orders.counterSale.decrease", { name })}
                increaseLabel={translate("orders.counterSale.increase", { name })}
              />
            );
          })}
        </Card>
      ))}
      {orderProblem ? (
        <Banner
          tone="warning"
          message={translate(orderProblemMessages[orderProblem] as TranslationKey)}
        />
      ) : null}
      {lines.some((line) => line.productVariantId !== null) && shop.deliveryClasses.length > 0 ? (
        <DeliveryPicker
          deliveryClasses={shop.deliveryClasses}
          classGroupId={deliveryClassGroupId}
          onChange={setDeliveryClassGroupId}
        />
      ) : null}
      {lines.length > 0 ? (
        <View className="gap-2">
          <AppText variant="caption" tone="subtle">
            {translate("family.shop.payAtSchool")}
          </AppText>
          <Button
            label={translate("family.shop.order", {
              total: formatMoney(total, shop.currencyCode),
            })}
            onPress={placeOrder}
            isLoading={placeFamilyOrderMutation.isPending}
          />
        </View>
      ) : null}
    </ScrollScreen>
  );
}
