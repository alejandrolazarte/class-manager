import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Redirect, useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { isBackofficeSessionEnded } from "@/backoffice/api/backofficeHttpClient";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { brandQueryKey } from "@/backoffice/brands/brandQueryKeys";
import { BrandDetail, getBrand } from "@/backoffice/brands/brandsApi";
import { amountText, applySubmissionError } from "@/backoffice/brands/subscriptionFormErrors";
import { brandSalesFeeQueryKey } from "@/backoffice/salesFees/salesFeeQueryKeys";
import {
  BrandSalesFeeFormValues,
  brandSalesFeeSchema,
  parsePercent,
} from "@/backoffice/salesFees/salesFeeSchema";
import {
  BrandSalesFee,
  getBrandSalesFee,
  giveBrandSalesFee,
} from "@/backoffice/salesFees/salesFeesApi";
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

const formFieldNames = ["percent", "note"] as const;

interface BrandSalesFeeScreenProps {
  brandId: string;
}

interface BrandSalesFeeFormProps {
  brand: BrandDetail;
  salesFee: BrandSalesFee;
}

function BrandSalesFeeForm({ brand, salesFee }: BrandSalesFeeFormProps) {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const [submissionMessage, setSubmissionMessage] = useState<string | null>(null);
  const form = useForm<BrandSalesFeeFormValues>({
    resolver: zodResolver(brandSalesFeeSchema),
    defaultValues: {
      percent: salesFee.isBrandFee ? amountText(salesFee.percent) : "",
      note: salesFee.note ?? "",
    },
    mode: "onTouched",
  });
  const areRequiredFieldsFilled = useRequiredFieldsFilled(form.control, ["percent"]);
  const give = useMutation({
    mutationFn: (values: BrandSalesFeeFormValues) =>
      giveBrandSalesFee(brand.id, {
        percent: parsePercent(values.percent) ?? 0,
        note: values.note.trim() === "" ? null : values.note.trim(),
      }),
  });
  const brandRoute = backofficeRoutes.brand(brand.id);

  const [valuesToSave, setValuesToSave] = useState<BrandSalesFeeFormValues | null>(null);

  const save = async (values: BrandSalesFeeFormValues) => {
    setValuesToSave(null);
    setSubmissionMessage(null);
    try {
      const updatedSalesFee = await give.mutateAsync(values);
      queryClient.setQueryData(brandSalesFeeQueryKey(brand.id), updatedSalesFee);
      showToast(translate("backoffice.salesFee.saved"));
      router.replace(brandRoute);
    } catch (error) {
      setSubmissionMessage(applySubmissionError(form, error, formFieldNames));
    }
  };

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          title={translate("backoffice.salesFee.give")}
          subtitle={brand.name}
          navigation="close"
          onNavigate={() => router.replace(brandRoute)}
        />
      }
      footer={
        <ScreenFooter>
          <Button
            label={translate("backoffice.salesFee.submit")}
            onPress={form.handleSubmit(setValuesToSave)}
            disabled={!areRequiredFieldsFilled}
            isLoading={give.isPending}
          />
        </ScreenFooter>
      }
    >
      <RequiredFieldsLegend />
      <AppText variant="body" tone="muted">
        {translate("backoffice.salesFee.explanation", {
          planPercent: formatPercent(salesFee.planPercent),
        })}
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
      <Controller
        control={form.control}
        name="note"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("backoffice.subscriptionTerms.note")}
            hint={translate("backoffice.salesFee.noteHint")}
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
          title={translate("backoffice.salesFee.confirmQuestion", {
            percent: formatPercent(parsePercent(valuesToSave.percent) ?? 0),
          })}
          message={translate("backoffice.salesFee.confirmMessage")}
          confirmLabel={translate("backoffice.salesFee.confirm")}
          onCancel={() => setValuesToSave(null)}
          onConfirm={() => save(valuesToSave)}
          isConfirming={give.isPending}
        />
      )}
    </ScrollScreen>
  );
}

export function BrandSalesFeeScreen({ brandId }: BrandSalesFeeScreenProps) {
  const brand = useQuery({
    queryKey: brandQueryKey(brandId),
    queryFn: () => getBrand(brandId),
    retry: false,
  });
  const salesFee = useQuery({
    queryKey: brandSalesFeeQueryKey(brandId),
    queryFn: () => getBrandSalesFee(brandId),
    retry: false,
  });

  if (isBackofficeSessionEnded(brand.error) || isBackofficeSessionEnded(salesFee.error)) {
    return <Redirect href={backofficeRoutes.signIn} />;
  }
  if (brand.isError || salesFee.isError) {
    return (
      <ScrollScreen
        header={<ScreenHeader title={translate("backoffice.salesFee.give")} navigation="back" />}
      >
        <Banner message={translate("common.unexpectedError")} />
      </ScrollScreen>
    );
  }
  if (brand.data === undefined || salesFee.data === undefined) {
    return <SkeletonScreen />;
  }
  return <BrandSalesFeeForm brand={brand.data} salesFee={salesFee.data} />;
}
