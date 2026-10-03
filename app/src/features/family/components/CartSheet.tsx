import { useState } from "react";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { QuantityStepper } from "@/features/family/components/QuantityStepper";
import { useFamilyCart } from "@/features/family/FamilyCartProvider";
import { cartItems, cartLines, cartTotal, maximumUnitsPerItem } from "@/features/family/familyCart";
import { familyErrorCodes } from "@/features/family/familyErrorCodes";
import { FamilyShop } from "@/features/family/types";
import { usePlaceFamilyOrder } from "@/features/family/useFamilyShop";
import { formatMoney } from "@/features/fees/money";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { BottomSheet } from "@/ui/BottomSheet";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
import { IconButton } from "@/ui/IconButton";
import { OptionCard } from "@/ui/OptionCard";

type OrderProblem = "outOfStock" | "tooManyOpenOrders" | "unexpected";

const orderProblemMessages: Record<OrderProblem, TranslationKey> = {
  outOfStock: "family.shop.outOfStock",
  tooManyOpenOrders: "family.shop.tooManyOpenOrders",
  unexpected: "common.unexpectedError",
};

function orderProblemOf(orderError: unknown): OrderProblem {
  if (isApiError(orderError) && orderError.hasCode(familyErrorCodes.outOfStock)) {
    return "outOfStock";
  }
  if (isApiError(orderError) && orderError.hasCode(familyErrorCodes.tooManyOpenOrders)) {
    return "tooManyOpenOrders";
  }
  return "unexpected";
}

interface CartSheetProps {
  shop: FamilyShop;
  onClose: () => void;
  onSeeOrders: () => void;
}

function OrderPlaced({ onClose, onSeeOrders }: Omit<CartSheetProps, "shop">) {
  return (
    <View className="items-center gap-2.5 pb-2 pt-6">
      <View className="h-[72px] w-[72px] items-center justify-center rounded-full bg-success-soft">
        <Icon name="present" size="huge" tone="success" />
      </View>
      <AppText variant="headline" accessibilityRole="header">
        {translate("family.cart.ordered")}
      </AppText>
      <AppText variant="body" tone="muted" className="max-w-[280px] text-center">
        {translate("family.cart.orderedBody")}
      </AppText>
      <View className="mt-2 w-full gap-2">
        <Button label={translate("family.cart.seeOrders")} onPress={onSeeOrders} />
        <Button
          variant="secondary"
          label={translate("family.cart.keepShopping")}
          onPress={onClose}
        />
      </View>
    </View>
  );
}

export function CartSheet({ shop, onClose, onSeeOrders }: CartSheetProps) {
  const { cart, setUnits, clear } = useFamilyCart();
  const placeFamilyOrderMutation = usePlaceFamilyOrder();
  const [deliveryClassGroupId, setDeliveryClassGroupId] = useState<string | null>(null);
  const [orderProblem, setOrderProblem] = useState<OrderProblem | null>(null);
  const [isOrdered, setIsOrdered] = useState(false);

  if (isOrdered) {
    return (
      <BottomSheet onClose={onClose}>
        <OrderPlaced onClose={onClose} onSeeOrders={onSeeOrders} />
      </BottomSheet>
    );
  }

  const items = cartItems(shop, cart);
  const hasProducts = items.some((item) => !item.isPack);
  const total = formatMoney(cartTotal(shop, cart), shop.currencyCode);

  const placeOrder = async () => {
    setOrderProblem(null);
    try {
      await placeFamilyOrderMutation.mutateAsync({
        lines: cartLines(shop, cart),
        delivery: hasProducts
          ? {
              delivery: deliveryClassGroupId === null ? "Pickup" : "InClass",
              deliveryClassGroupId,
            }
          : null,
      });
      clear();
      setIsOrdered(true);
    } catch (orderError) {
      setOrderProblem(orderProblemOf(orderError));
    }
  };

  return (
    <BottomSheet onClose={onClose}>
      <View className="flex-row items-center justify-between">
        <AppText variant="headline" accessibilityRole="header">
          {translate("family.cart.title")}
        </AppText>
        <IconButton
          icon="close"
          tone="muted-foreground"
          accessibilityLabel={translate("common.close")}
          onPress={onClose}
        />
      </View>
      <Card className="px-3.5 py-1">
        {items.map((item, index) => (
          <View
            key={item.itemId}
            className={`flex-row items-center gap-3 py-2.5 ${index < items.length - 1 ? "border-b border-border-subtle" : ""}`}
          >
            <View className="h-12 w-12 items-center justify-center rounded-xl bg-muted">
              <Icon name={item.isPack ? "classPacks" : "products"} tone="primary" />
            </View>
            <View className="min-w-0 flex-1">
              <AppText variant="bodyStrong">{item.name}</AppText>
              <AppText variant="caption" tone="subtle">
                {formatMoney(item.total, shop.currencyCode)}
              </AppText>
            </View>
            <QuantityStepper
              units={item.units}
              minimum={0}
              maximum={item.isPack ? 1 : maximumUnitsPerItem}
              onChange={(units) => setUnits(item.itemId, units)}
              decreaseLabel={translate("family.cart.decrease", { name: item.name })}
              increaseLabel={translate("family.cart.increase", { name: item.name })}
            />
          </View>
        ))}
      </Card>
      {hasProducts ? (
        <View className="gap-2">
          <AppText variant="label" tone="muted">
            {translate("delivery.title")}
          </AppText>
          <OptionCard
            label={translate("delivery.pickup")}
            hint={translate("family.cart.pickupHint")}
            isSelected={deliveryClassGroupId === null}
            onPress={() => setDeliveryClassGroupId(null)}
          />
          {shop.deliveryClasses.map((deliveryClass) => (
            <OptionCard
              key={`${deliveryClass.classGroupId}-${deliveryClass.studentFullName}`}
              label={translate("delivery.inClass", {
                className: deliveryClass.classGroupName,
                student: deliveryClass.studentFullName,
              })}
              hint={translate("family.cart.inClassHint")}
              isSelected={deliveryClassGroupId === deliveryClass.classGroupId}
              onPress={() => setDeliveryClassGroupId(deliveryClass.classGroupId)}
            />
          ))}
        </View>
      ) : null}
      <View className="gap-2">
        <AppText variant="label" tone="muted">
          {translate("family.cart.payment")}
        </AppText>
        <View className="flex-row items-center gap-2.5 rounded-2xl border-[1.5px] border-primary bg-surface px-3.5 py-3">
          <Icon name="business" tone="primary" />
          <View className="min-w-0 flex-1">
            <AppText variant="bodyStrong">{translate("family.cart.payAtSchool")}</AppText>
            <AppText variant="caption" tone="subtle">
              {translate("family.cart.payAtSchoolHint")}
            </AppText>
          </View>
        </View>
      </View>
      <View className="flex-row justify-between px-0.5 py-1">
        <AppText variant="headline">{translate("family.cart.total")}</AppText>
        <AppText variant="headline">{total}</AppText>
      </View>
      {orderProblem ? (
        <Banner tone="warning" message={translate(orderProblemMessages[orderProblem])} />
      ) : null}
      <Button
        icon="business"
        label={translate("family.shop.order", { total })}
        disabled={items.length === 0}
        onPress={placeOrder}
        isLoading={placeFamilyOrderMutation.isPending}
      />
      <AppText variant="footnote" tone="subtle" className="text-center">
        {translate("family.shop.payAtSchool")}
      </AppText>
    </BottomSheet>
  );
}
