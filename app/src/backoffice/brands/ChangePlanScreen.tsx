import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Redirect, useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { View } from "react-native";
import { isBackofficeSessionEnded } from "@/backoffice/api/backofficeHttpClient";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { featureLabel, planLabel } from "@/backoffice/brands/brandLabels";
import {
  brandQueryKey,
  brandsQueryKey,
  subscriptionCatalogQueryKey,
} from "@/backoffice/brands/brandQueryKeys";
import { BrandDetail, getBrand } from "@/backoffice/brands/brandsApi";
import {
  CatalogPlan,
  changeBrandPlan,
  getSubscriptionCatalog,
  SubscriptionCatalog,
} from "@/backoffice/brands/brandSubscriptionApi";
import { amountText, applySubmissionError } from "@/backoffice/brands/subscriptionFormErrors";
import {
  ChangePlanFormValues,
  changePlanSchema,
  toChangePlanRequest,
} from "@/backoffice/brands/subscriptionTermsSchema";
import { SubscriptionTermsFields } from "@/backoffice/brands/SubscriptionTermsFields";
import { formatMoney } from "@/formatting/money";
import { useRequiredFieldsFilled } from "@/forms/requiredFields";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
import { OptionCard } from "@/ui/OptionCard";
import { RequiredFieldsLegend, withRequiredMark } from "@/ui/RequiredFieldsLegend";
import { ScreenFooter, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SkeletonScreen } from "@/ui/SkeletonScreen";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

const formFieldNames = ["planCode", "price", "currency", "note", "expiredOn"] as const;
const defaultCurrency = "EUR";

interface ChangePlanScreenProps {
  brandId: string;
}

function planHint(plan: CatalogPlan, currentPlanCode: string | undefined): string {
  if (plan.code === currentPlanCode) {
    return translate("backoffice.changePlan.currentPlan");
  }
  if (plan.durationInDays !== null) {
    return translate("subscriptions.trialDuration", { days: plan.durationInDays });
  }
  return plan.listPrice === null
    ? translate("subscriptions.priceOnRequest")
    : translate("subscriptions.pricePerMonth", {
        price: formatMoney(plan.listPrice, plan.currency),
      });
}

interface ChangePlanFormProps {
  brand: BrandDetail;
  catalog: SubscriptionCatalog;
}

function ChangePlanForm({ brand, catalog }: ChangePlanFormProps) {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const [confirmedValues, setConfirmedValues] = useState<ChangePlanFormValues | null>(null);
  const [submissionMessage, setSubmissionMessage] = useState<string | null>(null);
  const form = useForm<ChangePlanFormValues>({
    resolver: zodResolver(changePlanSchema),
    defaultValues: {
      planCode: brand.subscription?.planCode ?? "",
      price: amountText(brand.subscription?.price ?? null),
      currency: brand.subscription?.currency ?? defaultCurrency,
      note: "",
      expiredOn: "",
    },
    mode: "onTouched",
  });
  const areRequiredFieldsFilled = useRequiredFieldsFilled(form.control, [
    "planCode",
    "price",
    "currency",
  ]);
  const changePlan = useMutation({
    mutationFn: (values: ChangePlanFormValues) =>
      changeBrandPlan(brand.id, toChangePlanRequest(values)),
  });
  const brandRoute = backofficeRoutes.brand(brand.id);
  const lostAddOns = brand.addOns.map((addOn) => featureLabel(addOn.featureCode)).join(", ");
  const confirmedPlan = confirmedValues === null ? "" : planLabel(confirmedValues.planCode);

  const choosePlan = (planCode: string) => {
    form.setValue("planCode", planCode);
    const plan = catalog.plans.find((candidate) => candidate.code === planCode);
    if (plan?.listPrice != null) {
      form.setValue("price", amountText(plan.listPrice), { shouldValidate: true });
      form.setValue("currency", plan.currency, { shouldValidate: true });
    }
  };

  const confirm = async () => {
    if (confirmedValues === null) {
      return;
    }
    setSubmissionMessage(null);
    try {
      await changePlan.mutateAsync(confirmedValues);
      await queryClient.invalidateQueries({ queryKey: brandsQueryKey });
      showToast(translate("backoffice.changePlan.saved"));
      router.replace(brandRoute);
    } catch (error) {
      setSubmissionMessage(applySubmissionError(form, error, formFieldNames));
    } finally {
      setConfirmedValues(null);
    }
  };

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          title={translate("backoffice.changePlan.title")}
          subtitle={
            brand.subscription === null
              ? brand.name
              : translate("backoffice.changePlan.subtitle", {
                  brand: brand.name,
                  plan: planLabel(brand.subscription.planCode),
                })
          }
          navigation="close"
          onNavigate={() => router.replace(brandRoute)}
        />
      }
      footer={
        <ScreenFooter>
          <Button
            label={translate("backoffice.changePlan.submit")}
            onPress={form.handleSubmit(setConfirmedValues)}
            disabled={!areRequiredFieldsFilled}
            isLoading={changePlan.isPending}
          />
        </ScreenFooter>
      }
    >
      <RequiredFieldsLegend />
      <Controller
        control={form.control}
        name="planCode"
        render={({ field }) => (
          <View className="gap-2" accessibilityRole="radiogroup">
            <AppText variant="label" tone="muted">
              {withRequiredMark(translate("backoffice.changePlan.plan"))}
            </AppText>
            {catalog.plans.map((plan) => (
              <OptionCard
                key={plan.code}
                label={planLabel(plan.code)}
                hint={planHint(plan, brand.subscription?.planCode)}
                isSelected={field.value === plan.code}
                onPress={() => choosePlan(plan.code)}
              />
            ))}
          </View>
        )}
      />
      <SubscriptionTermsFields control={form.control} />
      <Controller
        control={form.control}
        name="note"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("backoffice.subscriptionTerms.note")}
            hint={translate("backoffice.subscriptionTerms.noteHint")}
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      {lostAddOns === "" ? null : (
        <Banner
          tone="warning"
          message={translate("backoffice.changePlan.addOnsLost", { addOns: lostAddOns })}
        />
      )}
      {submissionMessage === null ? null : <Banner message={submissionMessage} />}
      {confirmedValues === null ? null : (
        <ConfirmDialog
          title={translate("backoffice.changePlan.confirmTitle", {
            brand: brand.name,
            plan: confirmedPlan,
          })}
          message={
            lostAddOns === ""
              ? translate("backoffice.changePlan.confirmMessage")
              : translate("backoffice.changePlan.confirmMessageWithAddOns", { addOns: lostAddOns })
          }
          confirmLabel={translate("backoffice.changePlan.confirm")}
          onCancel={() => setConfirmedValues(null)}
          onConfirm={confirm}
          isConfirming={changePlan.isPending}
        />
      )}
    </ScrollScreen>
  );
}

export function ChangePlanScreen({ brandId }: ChangePlanScreenProps) {
  const brand = useQuery({
    queryKey: brandQueryKey(brandId),
    queryFn: () => getBrand(brandId),
    retry: false,
  });
  const catalog = useQuery({
    queryKey: subscriptionCatalogQueryKey,
    queryFn: getSubscriptionCatalog,
    retry: false,
  });

  if (isBackofficeSessionEnded(brand.error) || isBackofficeSessionEnded(catalog.error)) {
    return <Redirect href={backofficeRoutes.signIn} />;
  }
  if (brand.isError || catalog.isError) {
    return (
      <ScrollScreen
        header={<ScreenHeader title={translate("backoffice.changePlan.title")} navigation="back" />}
      >
        <Banner message={translate("common.unexpectedError")} />
      </ScrollScreen>
    );
  }
  if (brand.data === undefined || catalog.data === undefined) {
    return <SkeletonScreen />;
  }
  return <ChangePlanForm brand={brand.data} catalog={catalog.data} />;
}
