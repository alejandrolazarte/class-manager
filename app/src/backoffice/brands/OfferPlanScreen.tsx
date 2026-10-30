import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Redirect, useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { View } from "react-native";
import { isBackofficeSessionEnded } from "@/backoffice/api/backofficeHttpClient";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { planLabel } from "@/backoffice/brands/brandLabels";
import {
  brandQueryKey,
  brandsQueryKey,
  subscriptionCatalogQueryKey,
} from "@/backoffice/brands/brandQueryKeys";
import { BrandDetail, getBrand } from "@/backoffice/brands/brandsApi";
import {
  getSubscriptionCatalog,
  offerBrandPlan,
  SubscriptionCatalog,
} from "@/backoffice/brands/brandSubscriptionApi";
import { amountText, applySubmissionError } from "@/backoffice/brands/subscriptionFormErrors";
import {
  OfferPlanFormValues,
  offerPlanSchema,
  toOfferPlanRequest,
} from "@/backoffice/brands/subscriptionTermsSchema";
import { todayIsoDate } from "@/dates/dates";
import { formatMoney } from "@/formatting/money";
import { DateField } from "@/forms/DateField";
import { useRequiredFieldsFilled } from "@/forms/requiredFields";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { OptionCard } from "@/ui/OptionCard";
import { RequiredFieldsLegend, withRequiredMark } from "@/ui/RequiredFieldsLegend";
import { ScreenFooter, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SkeletonScreen } from "@/ui/SkeletonScreen";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

const formFieldNames = ["planCode", "price", "note", "expiredOn"] as const;
const offerablePlanCodes = ["lite", "pro"];

interface OfferPlanScreenProps {
  brandId: string;
}

interface OfferPlanFormProps {
  brand: BrandDetail;
  catalog: SubscriptionCatalog;
}

function OfferPlanForm({ brand, catalog }: OfferPlanFormProps) {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const [submissionMessage, setSubmissionMessage] = useState<string | null>(null);
  const offerablePlans = catalog.plans.filter((plan) => offerablePlanCodes.includes(plan.code));
  const form = useForm<OfferPlanFormValues>({
    resolver: zodResolver(offerPlanSchema),
    defaultValues: {
      planCode: brand.offer?.planCode ?? "",
      price: amountText(brand.offer?.price ?? null),
      note: "",
      expiredOn: "",
    },
    mode: "onTouched",
  });
  const areRequiredFieldsFilled = useRequiredFieldsFilled(form.control, ["planCode", "price"]);
  const offerPlan = useMutation({
    mutationFn: (values: OfferPlanFormValues) =>
      offerBrandPlan(brand.id, toOfferPlanRequest(values)),
  });
  const brandRoute = backofficeRoutes.brand(brand.id);

  const choosePlan = (planCode: string) => {
    form.setValue("planCode", planCode);
    const plan = offerablePlans.find((candidate) => candidate.code === planCode);
    if (plan?.listPrice != null && form.getValues("price").trim() === "") {
      form.setValue("price", amountText(plan.listPrice), { shouldValidate: true });
    }
  };

  const submit = async (values: OfferPlanFormValues) => {
    setSubmissionMessage(null);
    try {
      await offerPlan.mutateAsync(values);
      await queryClient.invalidateQueries({ queryKey: brandsQueryKey });
      showToast(translate("backoffice.offerPlan.saved"));
      router.replace(brandRoute);
    } catch (error) {
      setSubmissionMessage(applySubmissionError(form, error, formFieldNames));
    }
  };

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          title={translate("backoffice.offerPlan.title")}
          subtitle={brand.name}
          navigation="close"
          onNavigate={() => router.replace(brandRoute)}
        />
      }
      footer={
        <ScreenFooter>
          <Button
            label={translate("backoffice.offerPlan.submit")}
            onPress={form.handleSubmit(submit)}
            disabled={!areRequiredFieldsFilled}
            isLoading={offerPlan.isPending}
          />
        </ScreenFooter>
      }
    >
      <RequiredFieldsLegend />
      <AppText variant="body" tone="muted">
        {translate("backoffice.offerPlan.explanation")}
      </AppText>
      <Controller
        control={form.control}
        name="planCode"
        render={({ field }) => (
          <View className="gap-2" accessibilityRole="radiogroup">
            <AppText variant="label" tone="muted">
              {withRequiredMark(translate("backoffice.changePlan.plan"))}
            </AppText>
            {offerablePlans.map((plan) => (
              <OptionCard
                key={plan.code}
                label={planLabel(plan.code)}
                hint={
                  plan.listPrice === null
                    ? undefined
                    : translate("subscriptions.pricePerMonthPlusVat", {
                        price: formatMoney(plan.listPrice, plan.currency),
                      })
                }
                isSelected={field.value === plan.code}
                onPress={() => choosePlan(plan.code)}
              />
            ))}
          </View>
        )}
      />
      <Controller
        control={form.control}
        name="price"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("backoffice.offerPlan.price")}
            hint={translate("backoffice.offerPlan.priceHint")}
            isRequired
            keyboardType="decimal-pad"
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <Controller
        control={form.control}
        name="expiredOn"
        render={({ field, fieldState }) => (
          <DateField
            label={translate("backoffice.offerPlan.expiredOn")}
            hint={translate("backoffice.offerPlan.expiredOnHint")}
            earliestIsoDate={todayIsoDate()}
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
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
      {brand.offer === null ? null : (
        <Banner tone="warning" message={translate("backoffice.offerPlan.replaces")} />
      )}
      {submissionMessage === null ? null : <Banner message={submissionMessage} />}
    </ScrollScreen>
  );
}

export function OfferPlanScreen({ brandId }: OfferPlanScreenProps) {
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
        header={<ScreenHeader title={translate("backoffice.offerPlan.title")} navigation="back" />}
      >
        <Banner message={translate("common.unexpectedError")} />
      </ScrollScreen>
    );
  }
  if (brand.data === undefined || catalog.data === undefined) {
    return <SkeletonScreen />;
  }
  return <OfferPlanForm brand={brand.data} catalog={catalog.data} />;
}
