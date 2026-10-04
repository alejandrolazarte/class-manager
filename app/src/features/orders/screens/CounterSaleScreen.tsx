import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { useClassPacks } from "@/features/classPacks/useClassPacks";
import { PaymentMethodPicker } from "@/features/fees/components/PaymentMethodPicker";
import { formatMoney } from "@/features/fees/money";
import { PaymentMethod } from "@/features/fees/types";
import { counterSaleLines, counterSaleTotal, maximumUnitsOf } from "@/features/orders/counterSale";
import { DeliveryPicker } from "@/features/studentApp/components/DeliveryPicker";
import { useCreateCounterSale } from "@/features/orders/useOrderMutations";
import { useDeliveryClasses } from "@/features/orders/useOrders";
import { productErrorCodes } from "@/features/products/productErrorCodes";
import { stockSummary, variantLabel } from "@/features/products/stockLabels";
import { useProducts } from "@/features/products/useProducts";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Chip } from "@/ui/Chip";
import { LoadingScreen } from "@/ui/LoadingScreen";
import { NumberStepper } from "@/ui/NumberStepper";
import { TextField } from "@/ui/TextField";
import { ToggleSwitch } from "@/ui/ToggleSwitch";
import { useToast } from "@/ui/ToastProvider";

interface CounterSaleScreenProps {
  clientId?: string;
}

type SaleProblem = "empty" | "outOfStock";

const saleProblemMessages = {
  empty: "orders.counterSale.empty",
  outOfStock: "orders.counterSale.outOfStock",
} as const;

export function CounterSaleScreen({ clientId }: CounterSaleScreenProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const currencyCode = useBusinessCurrency();
  const hasClient = clientId !== undefined;
  const { data: products = [], isPending: isLoadingProducts } = useProducts(false);
  const { data: classPacks = [] } = useClassPacks(false, { enabled: hasClient });
  const createCounterSaleMutation = useCreateCounterSale();
  const [classPackIds, setClassPackIds] = useState<string[]>([]);
  const [quantitiesByVariant, setQuantitiesByVariant] = useState<Record<string, string>>({});
  const [method, setMethod] = useState<PaymentMethod>("Cash");
  const [isDelivered, setIsDelivered] = useState(true);
  const [deliveryClassGroupId, setDeliveryClassGroupId] = useState<string | null>(null);
  const { data: deliveryClasses = [] } = useDeliveryClasses(clientId);
  const [notes, setNotes] = useState("");
  const [saleProblem, setSaleProblem] = useState<SaleProblem | null>(null);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const total = counterSaleTotal(classPacks, products, classPackIds, quantitiesByVariant);

  const togglePack = (classPackId: string) =>
    setClassPackIds(
      classPackIds.includes(classPackId)
        ? classPackIds.filter((selectedId) => selectedId !== classPackId)
        : [...classPackIds, classPackId],
    );

  const sell = async () => {
    setSaleProblem(null);
    setSubmissionFailure(null);
    const lines = counterSaleLines(classPackIds, quantitiesByVariant);
    if (lines.length === 0) {
      setSaleProblem("empty");
      return;
    }
    try {
      await createCounterSaleMutation.mutateAsync({
        clientId: clientId ?? null,
        lines,
        method,
        paidOn: null,
        notes: notes.trim().length === 0 ? null : notes.trim(),
        isDelivered,
        delivery: isDelivered ? null : deliveryClassGroupId === null ? "Pickup" : "InClass",
        deliveryClassGroupId: isDelivered ? null : deliveryClassGroupId,
      });
      showToast(translate("orders.counterSale.saved"));
      router.back();
    } catch (saleError) {
      if (isApiError(saleError) && saleError.hasCode(productErrorCodes.outOfStock)) {
        setSaleProblem("outOfStock");
        return;
      }
      setSubmissionFailure(toSubmissionFailure(saleError));
    }
  };

  if (isLoadingProducts) {
    return <LoadingScreen />;
  }

  return (
    <SettingsFormScreenLayout
      title={translate("orders.counterSale.title")}
      submissionFailure={submissionFailure}
      onRetry={sell}
    >
      {hasClient && classPacks.length > 0 ? (
        <View className="gap-2">
          <AppText variant="label" tone="muted">
            {translate("orders.counterSale.packs")}
          </AppText>
          <View className="flex-row flex-wrap gap-2">
            {classPacks.map((classPack) => (
              <Chip
                key={classPack.id}
                label={`${classPack.name} · ${formatMoney(classPack.price, currencyCode)}`}
                accessibilityLabel={classPack.name}
                isSelected={classPackIds.includes(classPack.id)}
                onPress={() => togglePack(classPack.id)}
              />
            ))}
          </View>
        </View>
      ) : null}
      {products.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate("orders.counterSale.noProducts")}
        </AppText>
      ) : (
        products.map((product) => (
          <Card key={product.id} className="gap-2 p-4">
            <AppText variant="bodyStrong">
              {`${product.name} · ${formatMoney(product.price, currencyCode)}`}
            </AppText>
            <AppText variant="caption" tone="subtle">
              {stockSummary(product)}
            </AppText>
            {product.variants.map((variant) => {
              const label =
                product.variants.length > 1
                  ? `${variantLabel(variant)}${variant.stock === null ? "" : ` (${variant.stock})`}`
                  : product.name;
              return (
                <NumberStepper
                  key={variant.id}
                  label={label}
                  value={quantitiesByVariant[variant.id] ?? "0"}
                  onChange={(quantityText) =>
                    setQuantitiesByVariant({ ...quantitiesByVariant, [variant.id]: quantityText })
                  }
                  minimum={0}
                  maximum={maximumUnitsOf(product, variant)}
                  decreaseLabel={translate("orders.counterSale.decrease", { name: label })}
                  increaseLabel={translate("orders.counterSale.increase", { name: label })}
                />
              );
            })}
          </Card>
        ))
      )}
      <PaymentMethodPicker value={method} onChange={setMethod} />
      <ToggleSwitch
        label={translate("orders.counterSale.handedOver")}
        value={isDelivered}
        onValueChange={setIsDelivered}
      />
      {!isDelivered && deliveryClasses.length > 0 ? (
        <DeliveryPicker
          deliveryClasses={deliveryClasses}
          classGroupId={deliveryClassGroupId}
          onChange={setDeliveryClassGroupId}
        />
      ) : null}
      <TextField
        label={translate("orders.counterSale.notes")}
        value={notes}
        onChangeText={setNotes}
      />
      {saleProblem ? (
        <Banner tone="warning" message={translate(saleProblemMessages[saleProblem])} />
      ) : null}
      <Button
        label={translate("orders.counterSale.submit", { total: formatMoney(total, currencyCode) })}
        onPress={sell}
        isLoading={createCounterSaleMutation.isPending}
      />
    </SettingsFormScreenLayout>
  );
}
