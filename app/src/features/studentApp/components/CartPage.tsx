import { useState } from "react";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { CatalogImage } from "@/features/catalogImages/components/CatalogImage";
import { QuantityStepper } from "@/features/studentApp/components/QuantityStepper";
import { groupDeliveryClasses } from "@/features/studentApp/deliveryClassGroups";
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
import { formatMoney } from "@/formatting/money";
import { translate, translateCount, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { EmptyState } from "@/ui/EmptyState";
import { FullScreenPage } from "@/ui/FullScreenPage";
import { Icon } from "@/ui/Icon";
import { OptionCard } from "@/ui/OptionCard";
import { HairlineRow } from "@/ui/HairlineRow";
import { TotalBar } from "@/ui/TotalBar";
import { AnimatedListItem } from "@/ui/motion/AnimatedListItem";
import { Entrance } from "@/ui/motion/Entrance";

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
    <View className="items-center gap-2.5 px-5 pb-2 pt-12">
      <Entrance kind="pop">
        <View className="h-[72px] w-[72px] items-center justify-center rounded-full bg-success-soft">
          <Icon name="present" size="huge" tone="success" />
        </View>
      </Entrance>
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
    <HairlineRow className="flex-row gap-3 py-3">
      <CatalogImage
        imageUri={item.imageUrl}
        placeholderIcon={item.isPack ? "classPacks" : "products"}
        className="h-[72px] w-[72px] rounded-xl"
      />
      <View className="min-w-0 flex-1 justify-between gap-1.5">
        <View className="flex-row justify-between gap-2">
          <View className="min-w-0 flex-1 gap-0.5">
            <AppText variant="bodyStrong">{item.title}</AppText>
            <AppText variant="caption" tone="subtle">
              {cartItemSubtitle(item, currencyCode)}
            </AppText>
          </View>
          <AppText variant="bodyStrong">{formatMoney(item.total, currencyCode)}</AppText>
        </View>
        <View className="self-start rounded-[10px] border border-border bg-surface">
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
    </HairlineRow>
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
          <TotalBar total={total}>
            {orderProblem ? (
              <Banner tone="warning" message={translate(orderProblemMessages[orderProblem])} />
            ) : null}
            <Button
              icon="products"
              label={translate("student.shop.order")}
              accessibilityLabel={`${translate("student.shop.order")} ${total}`}
              onPress={placeOrder}
              isLoading={placeStudentAppOrderMutation.isPending}
            />
          </TotalBar>
        ) : null
      }
    >
      {hasItems ? (
        <View className="gap-5 px-5 pb-5 pt-1">
          <View>
            {items.map((item, index) => (
              <AnimatedListItem key={item.itemId} index={index}>
                <CartLine
                  item={item}
                  currencyCode={shop.currencyCode}
                  onChangeUnits={(units) => setUnits(item.itemId, units)}
                />
              </AnimatedListItem>
            ))}
          </View>
          {hasProducts ? (
            <View>
              <AppText variant="label" tone="muted">
                {translate("delivery.title")}
              </AppText>
              <OptionCard
                label={translate("delivery.pickup")}
                hint={translate("student.cart.pickupHint")}
                isSelected={deliveryClassGroupId === null}
                onPress={() => setDeliveryClassGroupId(null)}
              />
              {groupDeliveryClasses(shop.deliveryClasses).map((deliveryClass) => (
                <OptionCard
                  key={deliveryClass.classGroupId}
                  label={translate("delivery.inClass", {
                    className: deliveryClass.classGroupName,
                    student: deliveryClass.studentNames.join(", "),
                  })}
                  hint={translate("student.cart.inClassHint")}
                  isSelected={deliveryClassGroupId === deliveryClass.classGroupId}
                  onPress={() => setDeliveryClassGroupId(deliveryClass.classGroupId)}
                />
              ))}
            </View>
          ) : null}
          <View>
            <AppText variant="label" tone="muted">
              {translate("student.cart.payment")}
            </AppText>
            <OptionCard
              label={translate("student.cart.payAtSchool")}
              hint={translate("student.cart.payAtSchoolHint")}
              isSelected
            />
          </View>
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
