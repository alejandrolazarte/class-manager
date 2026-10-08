import { useRouter } from "expo-router";
import { useEffect, useState } from "react";
import { BackHandler, View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { useClassPacks } from "@/features/classPacks/useClassPacks";
import { formatPhoneNumberForDisplay } from "@/features/clients/phoneNumberFormatting";
import { useClient } from "@/features/clients/useClient";
import { PaymentMethodPicker } from "@/features/fees/components/PaymentMethodPicker";
import { formatMoney } from "@/features/fees/money";
import { PaymentMethod } from "@/features/fees/types";
import { CounterSaleCatalogPicker } from "@/features/orders/components/CounterSaleCatalogPicker";
import { CounterSaleItemRow } from "@/features/orders/components/CounterSaleItemRow";
import {
  chosenItems,
  CounterSaleItem,
  counterSaleItems,
  counterSaleLines,
  counterSaleTotal,
  deliveryClassOptions,
  UnitsByItem,
  unitsOf,
  withoutPacks,
} from "@/features/orders/counterSale";
import { useCreateCounterSale } from "@/features/orders/useOrderMutations";
import { useDeliveryClasses } from "@/features/orders/useOrders";
import { productErrorCodes } from "@/features/products/productErrorCodes";
import { useProducts } from "@/features/products/useProducts";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { StudentPicker, studentContactLine } from "@/features/students/components/StudentPicker";
import { StudentSummary } from "@/features/students/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
import { LoadingScreen } from "@/ui/LoadingScreen";
import { OptionCard } from "@/ui/OptionCard";
import { PickerField } from "@/ui/PickerField";
import { StepScreen } from "@/ui/StepScreen";
import { TextField } from "@/ui/TextField";
import { ToggleSwitch } from "@/ui/ToggleSwitch";
import { TotalBar } from "@/ui/TotalBar";
import { useToast } from "@/ui/ToastProvider";

interface CounterSaleScreenProps {
  clientId?: string;
}

interface SaleCustomer {
  clientId: string;
  name: string;
  detail: string;
}

type SaleStep = 1 | 2;

type SalePicker = "student" | "catalog";

const saleStepCount = 2;
const emptySaleFill = 0.4;

function customerOfStudent(student: StudentSummary): SaleCustomer {
  return {
    clientId: student.clientId,
    name: student.fullName,
    detail: studentContactLine(student),
  };
}

export function CounterSaleScreen({ clientId }: CounterSaleScreenProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const currencyCode = useBusinessCurrency();
  const { data: products = [], isPending: isLoadingProducts } = useProducts(false);
  const { data: classPacks = [] } = useClassPacks(false);
  const { data: initialClient } = useClient(clientId);
  const createCounterSaleMutation = useCreateCounterSale();
  const [step, setStep] = useState<SaleStep>(1);
  const [openPicker, setOpenPicker] = useState<SalePicker | null>(null);
  const [pickedCustomer, setPickedCustomer] = useState<SaleCustomer | null | undefined>();
  const [unitsByItem, setUnitsByItem] = useState<UnitsByItem>({});
  const [method, setMethod] = useState<PaymentMethod>("Cash");
  const [isDelivered, setIsDelivered] = useState(true);
  const [deliveryClassGroupId, setDeliveryClassGroupId] = useState<string | null>(null);
  const [notes, setNotes] = useState("");
  const [isOutOfStock, setIsOutOfStock] = useState(false);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const initialCustomer: SaleCustomer | null =
    clientId === undefined
      ? null
      : {
          clientId,
          name: initialClient?.fullName ?? "",
          detail: initialClient ? formatPhoneNumberForDisplay(initialClient.phoneNumber) : "",
        };
  const customer = pickedCustomer === undefined ? initialCustomer : pickedCustomer;
  const { data: deliveryClasses = [] } = useDeliveryClasses(customer?.clientId);
  const items = counterSaleItems(classPacks, products);
  const saleItems = chosenItems(items, unitsByItem);
  const total = formatMoney(counterSaleTotal(items, unitsByItem), currencyCode);
  const hasItems = saleItems.length > 0;
  const hasProducts = saleItems.some((item) => item.kind === "product");

  const changeUnits = (item: CounterSaleItem, units: number) =>
    setUnitsByItem({ ...unitsByItem, [item.key]: Math.max(0, units) });

  const chooseCustomer = (nextCustomer: SaleCustomer | null) => {
    setPickedCustomer(nextCustomer);
    setDeliveryClassGroupId(null);
    if (nextCustomer === null) {
      setUnitsByItem(withoutPacks(items, unitsByItem));
    }
  };

  const goBack = () => (step === 2 ? setStep(1) : router.back());

  useEffect(() => {
    if (step === 1) {
      return undefined;
    }
    const subscription = BackHandler.addEventListener("hardwareBackPress", () => {
      setStep(1);
      return true;
    });
    return () => subscription.remove();
  }, [step]);

  const sell = async () => {
    setIsOutOfStock(false);
    setSubmissionFailure(null);
    try {
      await createCounterSaleMutation.mutateAsync({
        clientId: customer?.clientId ?? null,
        lines: counterSaleLines(items, unitsByItem),
        method,
        paidOn: null,
        notes: notes.trim().length === 0 ? null : notes.trim(),
        isDelivered: !hasProducts || isDelivered,
        delivery:
          !hasProducts || isDelivered ? null : deliveryClassGroupId === null ? "Pickup" : "InClass",
        deliveryClassGroupId: !hasProducts || isDelivered ? null : deliveryClassGroupId,
      });
      showToast(translate("orders.counterSale.saved"));
      router.back();
    } catch (saleError) {
      if (isApiError(saleError) && saleError.hasCode(productErrorCodes.outOfStock)) {
        setIsOutOfStock(true);
        return;
      }
      setSubmissionFailure(toSubmissionFailure(saleError));
    }
  };

  if (isLoadingProducts) {
    return <LoadingScreen />;
  }

  const pickers =
    openPicker === "student" ? (
      <StudentPicker
        testID="counter-sale-student-picker"
        onPick={(student) => {
          chooseCustomer(customerOfStudent(student));
          setOpenPicker(null);
        }}
        onClose={() => setOpenPicker(null)}
      />
    ) : openPicker === "catalog" ? (
      <CounterSaleCatalogPicker
        items={items}
        unitsByItem={unitsByItem}
        onChangeUnits={changeUnits}
        hasStudent={customer !== null}
        onClose={() => setOpenPicker(null)}
      />
    ) : null;

  if (step === 1) {
    return (
      <StepScreen
        step={1}
        stepCount={saleStepCount}
        title={translate("orders.counterSale.whatTitle")}
        segmentFills={[hasItems ? 1 : emptySaleFill, 0]}
        onBack={goBack}
        footer={
          <TotalBar total={total}>
            <Button
              label={translate("common.continue")}
              trailingIcon="forward"
              disabled={!hasItems}
              onPress={() => setStep(2)}
            />
          </TotalBar>
        }
      >
        <PickerField
          label={translate("orders.counterSale.student")}
          chooseLabel={translate("orders.counterSale.chooseStudent")}
          removeLabel={translate("orders.counterSale.removeStudent")}
          hint={translate("common.optional")}
          picked={customer}
          onChoose={() => setOpenPicker("student")}
          onRemove={() => chooseCustomer(null)}
        />
        <View className="gap-2">
          <AppText variant="label" tone="muted">
            {translate("orders.counterSale.items")}
          </AppText>
          {saleItems.map((item) => (
            <CounterSaleItemRow
              key={item.key}
              item={item}
              units={unitsOf(unitsByItem, item)}
              onChangeUnits={(units) => changeUnits(item, units)}
              showsLineTotal
            />
          ))}
          <Button
            variant="dashed"
            size="medium"
            icon="add"
            label={translate("orders.counterSale.addItems")}
            onPress={() => setOpenPicker("catalog")}
          />
        </View>
        {pickers}
      </StepScreen>
    );
  }

  return (
    <StepScreen
      step={2}
      stepCount={saleStepCount}
      title={translate("orders.counterSale.paymentTitle")}
      segmentFills={[1, 1]}
      onBack={goBack}
      footer={
        <TotalBar total={total}>
          {isOutOfStock ? (
            <Banner tone="warning" message={translate("orders.counterSale.outOfStock")} />
          ) : null}
          <Button
            icon="cash"
            label={translate("orders.counterSale.charge")}
            disabled={!hasItems}
            onPress={sell}
            isLoading={createCounterSaleMutation.isPending}
          />
        </TotalBar>
      }
    >
      {submissionFailure === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={sell}
          />
        </Banner>
      ) : null}
      {submissionFailure === "unexpected" ? (
        <Banner message={translate("common.unexpectedError")} />
      ) : null}
      {hasProducts ? (
        <View className="gap-2">
          <ToggleSwitch
            icon="handOver"
            label={translate("orders.counterSale.handedOver")}
            hint={translate(
              isDelivered
                ? "orders.counterSale.handedOverHint"
                : "orders.counterSale.pendingHandOverHint",
            )}
            value={isDelivered}
            onValueChange={setIsDelivered}
          />
          {isDelivered ? null : (
            <View className="gap-2" accessibilityRole="radiogroup">
              <OptionCard
                label={translate("delivery.pickup")}
                hint={translate("orders.counterSale.pickupHint")}
                isSelected={deliveryClassGroupId === null}
                onPress={() => setDeliveryClassGroupId(null)}
              />
              {deliveryClassOptions(deliveryClasses).map((deliveryClass) => (
                <OptionCard
                  key={deliveryClass.classGroupId}
                  label={translate("orders.counterSale.inClass")}
                  hint={deliveryClass.hint}
                  isSelected={deliveryClassGroupId === deliveryClass.classGroupId}
                  onPress={() => setDeliveryClassGroupId(deliveryClass.classGroupId)}
                />
              ))}
            </View>
          )}
        </View>
      ) : null}
      <PaymentMethodPicker
        label={translate("orders.counterSale.method")}
        value={method}
        onChange={setMethod}
      />
      <TextField
        label={translate("orders.counterSale.notes")}
        value={notes}
        onChangeText={setNotes}
      />
      <View className="gap-2">
        <AppText variant="label" tone="muted">
          {translate("orders.counterSale.summary")}
        </AppText>
        <Card className="gap-2.5 p-4">
          <View className="flex-row items-center gap-2.5">
            <Icon name="privateLesson" size="medium" tone="muted-foreground" />
            <AppText variant="bodyStrong" className="min-w-0 flex-1">
              {customer?.name || translate("orders.noClient")}
            </AppText>
          </View>
          {saleItems.map((item) => {
            const units = unitsOf(unitsByItem, item);
            return (
              <View
                key={item.key}
                className="flex-row items-center gap-2 border-t border-border pt-2.5"
              >
                <AppText variant="body" className="min-w-0 flex-1">
                  {translate("orders.counterSale.lineQuantity", {
                    quantity: units,
                    name: item.name,
                  })}
                </AppText>
                <AppText variant="bodyStrong">
                  {formatMoney(item.price * units, currencyCode)}
                </AppText>
              </View>
            );
          })}
        </Card>
      </View>
    </StepScreen>
  );
}
