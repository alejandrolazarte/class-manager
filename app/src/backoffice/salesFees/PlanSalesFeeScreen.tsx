import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Redirect, useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { isBackofficeSessionEnded } from "@/backoffice/api/backofficeHttpClient";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { planLabel } from "@/backoffice/brands/brandLabels";
import { brandsQueryKey, subscriptionCatalogQueryKey } from "@/backoffice/brands/brandQueryKeys";
import { CatalogPlan, getSubscriptionCatalog } from "@/backoffice/brands/brandSubscriptionApi";
import { amountText, applySubmissionError } from "@/backoffice/brands/subscriptionFormErrors";
import {
  parsePercent,
  PlanSalesFeeFormValues,
  planSalesFeeSchema,
} from "@/backoffice/salesFees/salesFeeSchema";
import { changePlanSalesFee } from "@/backoffice/salesFees/salesFeesApi";
import { formatPercent } from "@/formatting/percent";
import { useRequiredFieldsFilled } from "@/forms/requiredFields";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";
import { ScreenFooter, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SkeletonScreen } from "@/ui/SkeletonScreen";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

const formFieldNames = ["percent"] as const;

interface PlanSalesFeeScreenProps {
  planCode: string;
}

function PlanSalesFeeForm({ plan }: { plan: CatalogPlan }) {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const [submissionMessage, setSubmissionMessage] = useState<string | null>(null);
  const form = useForm<PlanSalesFeeFormValues>({
    resolver: zodResolver(planSalesFeeSchema),
    defaultValues: { percent: amountText(plan.salesFeePercent) },
    mode: "onTouched",
  });
  const areRequiredFieldsFilled = useRequiredFieldsFilled(form.control, ["percent"]);
  const change = useMutation({
    mutationFn: (values: PlanSalesFeeFormValues) =>
      changePlanSalesFee(plan.code, parsePercent(values.percent) ?? 0),
  });

  const [valuesToSave, setValuesToSave] = useState<PlanSalesFeeFormValues | null>(null);

  const save = async (values: PlanSalesFeeFormValues) => {
    setValuesToSave(null);
    setSubmissionMessage(null);
    try {
      await change.mutateAsync(values);
      await queryClient.invalidateQueries({ queryKey: subscriptionCatalogQueryKey });
      await queryClient.invalidateQueries({ queryKey: brandsQueryKey });
      showToast(translate("backoffice.plans.salesFeeSaved"));
      router.replace(backofficeRoutes.plans);
    } catch (error) {
      setSubmissionMessage(applySubmissionError(form, error, formFieldNames));
    }
  };

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          title={translate("backoffice.plans.changeSalesFeeOf", { plan: planLabel(plan.code) })}
          navigation="close"
          onNavigate={() => router.replace(backofficeRoutes.plans)}
        />
      }
      footer={
        <ScreenFooter>
          <Button
            label={translate("backoffice.plans.saveSalesFee")}
            onPress={form.handleSubmit(setValuesToSave)}
            disabled={!areRequiredFieldsFilled}
            isLoading={change.isPending}
          />
        </ScreenFooter>
      }
    >
      <RequiredFieldsLegend />
      <AppText variant="body" tone="muted">
        {translate("backoffice.plans.salesFeeChangeExplanation")}
      </AppText>
      <Controller
        control={form.control}
        name="percent"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("backoffice.salesFee.percent")}
            hint={translate("backoffice.salesFee.percentHint")}
            isRequired
            keyboardType="decimal-pad"
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      {submissionMessage === null ? null : <Banner message={submissionMessage} />}
      {valuesToSave === null ? null : (
        <ConfirmDialog
          title={translate("backoffice.plans.confirmQuestion", {
            plan: planLabel(plan.code),
            percent: formatPercent(parsePercent(valuesToSave.percent) ?? 0),
          })}
          message={translate("backoffice.plans.confirmMessage")}
          confirmLabel={translate("backoffice.plans.confirm")}
          onCancel={() => setValuesToSave(null)}
          onConfirm={() => save(valuesToSave)}
          isConfirming={change.isPending}
        />
      )}
    </ScrollScreen>
  );
}

export function PlanSalesFeeScreen({ planCode }: PlanSalesFeeScreenProps) {
  const catalog = useQuery({
    queryKey: subscriptionCatalogQueryKey,
    queryFn: getSubscriptionCatalog,
    retry: false,
  });

  if (isBackofficeSessionEnded(catalog.error)) {
    return <Redirect href={backofficeRoutes.signIn} />;
  }
  const plan = catalog.data?.plans.find((candidate) => candidate.code === planCode);
  if (catalog.isError || (catalog.data !== undefined && plan === undefined)) {
    return (
      <ScrollScreen
        header={<ScreenHeader title={translate("backoffice.plans.title")} navigation="back" />}
      >
        <Banner message={translate("common.unexpectedError")} />
      </ScrollScreen>
    );
  }
  if (plan === undefined) {
    return <SkeletonScreen />;
  }
  return <PlanSalesFeeForm plan={plan} />;
}
