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
import { useRequiredFieldsFilled } from "@/forms/requiredFields";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ChipGroup } from "@/ui/ChipGroup";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
import { RequiredFieldsLegend, withRequiredMark } from "@/ui/RequiredFieldsLegend";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SkeletonScreen } from "@/ui/SkeletonScreen";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

const formFieldNames = ["planCode", "price", "currency", "note", "expiredOn"] as const;
const defaultCurrency = "EUR";

interface ChangePlanScreenProps {
  brandId: string;
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
          eyebrow={brand.name}
          title={translate("backoffice.changePlan.title")}
          navigation="back"
          onNavigate={() => router.replace(brandRoute)}
        />
      }
    >
      <RequiredFieldsLegend />
      <Controller
        control={form.control}
        name="planCode"
        render={({ field }) => (
          <View className="gap-2">
            <AppText variant="bodyStrong">
              {withRequiredMark(translate("backoffice.changePlan.plan"))}
            </AppText>
            <ChipGroup
              options={catalog.plans.map((plan) => ({
                value: plan.code,
                label: planLabel(plan.code),
              }))}
              selectedValue={field.value}
              onChange={choosePlan}
            />
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
      {submissionMessage === null ? null : <Banner message={submissionMessage} />}
      <Button
        label={translate("backoffice.changePlan.submit")}
        onPress={form.handleSubmit(setConfirmedValues)}
        disabled={!areRequiredFieldsFilled}
        isLoading={changePlan.isPending}
      />
      {confirmedValues === null ? null : (
        <ConfirmDialog
          title={translate("backoffice.changePlan.confirmTitle", { brand: brand.name })}
          message={translate("backoffice.changePlan.confirmMessage")}
          confirmLabel={translate("backoffice.changePlan.submit")}
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
