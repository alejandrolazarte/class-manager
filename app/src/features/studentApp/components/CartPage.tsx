import { useState } from "react";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { CatalogImage } from "@/features/catalogImages/components/CatalogImage";
import { QuantityStepper } from "@/features/studentApp/components/QuantityStepper";
import { useStudentAppCart } from "@/features/studentApp/StudentAppCartProvider";
import {
  cartItems,
  cartLines,
  cartTotal,
  cartUnits,
  StudentAppCartItem,
  maximumUnitsPerItem,
} from "@/features/studentApp/studentAppCart";
import { studentAppErrorCodes } from "@/features/studentApp/studentAppErrorCodes";
import { StudentAppShop } from "@/features/studentApp/types";
import { usePlaceStudentAppOrder } from "@/features/studentApp/useStudentAppShop";
import { formatMoney } from "@/features/fees/money";
import { translate, translateCount, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { EmptyState } from "@/ui/EmptyState";
import { FullScreenPage } from "@/ui/FullScreenPage";
import { Icon } from "@/ui/Icon";
import { OptionCard } from "@/ui/OptionCard";

type OrderProblem = "outOfStock" | "tooManyOpenOrders" | "unexpected";

const orderProblemMessages: Record<OrderProblem, TranslationKey> = {
  outOfStock: "student.shop.outOfStock",
  tooManyOpenOrders: "student.shop.tooManyOpenOrders",
  unexpected: "common.unexpectedError",
};

function orderProblemOf(orderError: unknown): OrderProblem {
  if (isApiError(orderError) && orderError.hasCode(studentAppErrorCodes.outOfStock)) {
    return "outOfStock";
  }
  if (isApiError(orderError) && orderError.hasCode(studentAppErrorCodes.tooManyOpenOrders)) {
    return "tooManyOpenOrders";
  }
  return "unexpected";
}

interface CartPageProps {
  shop: StudentAppShop;
  onClose: () => void;
  onSeeOrders: () => void;
}

function OrderPlaced({ onClose, onSeeOrders }: Omit<CartPageProps, "shop">) {
  return (
    <View className="items-center gap-2.5 px-5 pb-2 pt-6">
      <View className="h-[72px] w-[72px] items-center justify-center rounded-full bg-success-soft">
        <Icon name="present" size="huge" tone="success" />
      </View>
      <AppText variant="headline" accessibilityRole="header">
        {translate("student.cart.ordered")}
      </AppText>
      <AppText variant="body" tone="muted" className="max-w-[280px] text-center">
        {translate("student.cart.orderedBody")}
      </AppText>
      <View className="mt-2 w-full gap-2">
        <Button label={translate("student.cart.seeOrders")} onPress={onSeeOrders} />
        <Button
          variant="secondary"
          label={translate("student.cart.keepShopping")}
          onPress={onClose}
        />
      </View>
    </View>
  );
}

function cartItemSubtitle(item: StudentAppCartItem, currencyCode: string): string {
  if (item.classCount !== null) {
    return translateCount("student.shop.classCount", item.classCount);
  }
  const price = formatMoney(item.unitPrice, currencyCode);
  return item.sizeName === null
    ? translate("student.cart.unitPrice", { price })
    : translate("student.cart.sizeAndUnitPrice", { size: item.sizeName, price });
}

interface CartLineProps {
  item: StudentAppCartItem;
  currencyCode: string;
  onChangeUnits: (units: number) => void;
}

function CartLine({ item, currencyCode, onChangeUnits }: CartLineProps) {
  const removesItem = item.units <= 1;
  return (
    <View className="flex-row gap-3 rounded-[20px] bg-surface p-2.5">
      <CatalogImage
        imageUri={item.imageUrl}
        placeholderIcon={item.isPack ? "classPacks" : "products"}
        className="h-[76px] w-[76px] rounded-[14px]"
      />
      <View className="min-w-0 flex-1 justify-between gap-1.5">
        <View className="flex-row justify-between gap-2">
          <View className="min-w-0 flex-1 gap-0.5">
            <AppText variant="bodyStrong">{item.title}</AppText>
            <AppText variant="caption" tone="subtle">
              {cartItemSubtitle(item, currencyCode)}
            </AppText>
          </View>
          <AppText variant="bodyStrong" className="font-heavy">
            {formatMoney(item.total, currencyCode)}
          </AppText>
        </View>
        <View className="self-start rounded-xl border-[1.5px] border-border">
          <QuantityStepper
            units={item.units}
            minimum={0}
            maximum={item.isPack ? 1 : maximumUnitsPerItem}
            onChange={onChangeUnits}
            decreaseIcon={removesItem ? "delete" : "remove"}
            decreaseLabel={translate(
              removesItem ? "student.cart.remove" : "student.cart.decrease",
              {
                name: item.name,
              },
            )}
            increaseLabel={translate("student.cart.increase", { name: item.name })}
          />
        </View>
      </View>
    </View>
  );
}

export function CartPage({ shop, onClose, onSeeOrders }: CartPageProps) {
  const { cart, setUnits, clear } = useStudentAppCart();
  const placeStudentAppOrderMutation = usePlaceStudentAppOrder();
  const [deliveryClassGroupId, setDeliveryClassGroupId] = useState<string | null>(null);
  const [orderProblem, setOrderProblem] = useState<OrderProblem | null>(null);
  const [isOrdered, setIsOrdered] = useState(false);
  const title = translate("student.cart.title");

  if (isOrdered) {
    return (
      <FullScreenPage title={title} onClose={onClose}>
        <OrderPlaced onClose={onClose} onSeeOrders={onSeeOrders} />
      </FullScreenPage>
    );
  }

  const items = cartItems(shop, cart);
  const hasItems = items.length > 0;
  const hasProducts = items.some((item) => !item.isPack);
  const total = formatMoney(cartTotal(shop, cart), shop.currencyCode);

  const placeOrder = async () => {
    setOrderProblem(null);
    try {
      await placeStudentAppOrderMutation.mutateAsync({
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
    <FullScreenPage
      title={title}
      onClose={onClose}
      accessory={
        hasItems ? (
          <AppText variant="label" tone="muted" className="pr-3">
            {translateCount("student.cart.itemCount", cartUnits(cart))}
          </AppText>
        ) : null
      }
      footer={
        hasItems ? (
          <View className="gap-2.5 rounded-t-3xl border-t border-border bg-surface px-5 pb-4 pt-3.5">
            <View className="flex-row items-baseline justify-between">
              <AppText variant="bodyStrong" tone="muted">
                {translate("student.cart.total")}
              </AppText>
              <AppText variant="display">{total}</AppText>
            </View>
            {orderProblem ? (
              <Banner tone="warning" message={translate(orderProblemMessages[orderProblem])} />
            ) : null}
            <Button
              icon="business"
              label={translate("student.shop.order", { total })}
              onPress={placeOrder}
              isLoading={placeStudentAppOrderMutation.isPending}
            />
          </View>
        ) : null
      }
    >
      {hasItems ? (
        <View className="gap-3.5 px-5 pb-5 pt-1">
          <View className="gap-2.5">
            {items.map((item) => (
              <CartLine
                key={item.itemId}
                item={item}
                currencyCode={shop.currencyCode}
                onChangeUnits={(units) => setUnits(item.itemId, units)}
              />
            ))}
          </View>
          {hasProducts ? (
            <View className="gap-2">
              <AppText variant="label" tone="muted">
                {translate("delivery.title")}
              </AppText>
              <OptionCard
                label={translate("delivery.pickup")}
                hint={translate("student.cart.pickupHint")}
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
                  hint={translate("student.cart.inClassHint")}
                  isSelected={deliveryClassGroupId === deliveryClass.classGroupId}
                  onPress={() => setDeliveryClassGroupId(deliveryClass.classGroupId)}
                />
              ))}
            </View>
          ) : null}
          <View className="gap-2">
            <AppText variant="label" tone="muted">
              {translate("student.cart.payment")}
            </AppText>
            <View className="flex-row items-center gap-2.5 rounded-2xl border-[1.5px] border-primary bg-surface px-3.5 py-3">
              <Icon name="business" tone="primary" />
              <View className="min-w-0 flex-1">
                <AppText variant="bodyStrong">{translate("student.cart.payAtSchool")}</AppText>
                <AppText variant="caption" tone="subtle">
                  {translate("student.cart.payAtSchoolHint")}
                </AppText>
              </View>
            </View>
          </View>
          <AppText variant="footnote" tone="subtle" className="text-center">
            {translate("student.shop.payAtSchool")}
          </AppText>
        </View>
      ) : (
        <EmptyState
          icon="products"
          iconTone="border-strong"
          message={translate("student.cart.empty")}
        />
      )}
    </FullScreenPage>
  );
}
