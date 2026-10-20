import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Redirect, useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm, useWatch } from "react-hook-form";
import { View } from "react-native";
import { isBackofficeSessionEnded } from "@/backoffice/api/backofficeHttpClient";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { featureLabel } from "@/backoffice/brands/brandLabels";
import {
  brandQueryKey,
  brandsQueryKey,
  subscriptionCatalogQueryKey,
} from "@/backoffice/brands/brandQueryKeys";
import { BrandDetail, getBrand } from "@/backoffice/brands/brandsApi";
import {
  getSubscriptionCatalog,
  giveBrandAddOn,
  SubscriptionCatalog,
} from "@/backoffice/brands/brandSubscriptionApi";
import { amountText, applySubmissionError } from "@/backoffice/brands/subscriptionFormErrors";
import {
  GiveAddOnFormValues,
  giveAddOnSchema,
  toGiveAddOnRequest,
} from "@/backoffice/brands/subscriptionTermsSchema";
import { SubscriptionTermsFields } from "@/backoffice/brands/SubscriptionTermsFields";
import { useRequiredFieldsFilled } from "@/forms/requiredFields";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ChipGroup } from "@/ui/ChipGroup";
import { RequiredFieldsLegend, withRequiredMark } from "@/ui/RequiredFieldsLegend";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SkeletonScreen } from "@/ui/SkeletonScreen";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

const formFieldNames = ["featureCode", "price", "currency", "limit", "expiredOn"] as const;
const defaultCurrency = "EUR";

interface GiveAddOnScreenProps {
  brandId: string;
}

interface GiveAddOnFormProps {
  brand: BrandDetail;
  catalog: SubscriptionCatalog;
}

function GiveAddOnForm({ brand, catalog }: GiveAddOnFormProps) {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const [submissionMessage, setSubmissionMessage] = useState<string | null>(null);
  const form = useForm<GiveAddOnFormValues>({
    resolver: zodResolver(giveAddOnSchema),
    defaultValues: {
      featureCode: "",
      price: "",
      currency: brand.subscription?.currency ?? defaultCurrency,
      limit: "",
      expiredOn: "",
    },
    mode: "onTouched",
  });
  const areRequiredFieldsFilled = useRequiredFieldsFilled(form.control, [
    "featureCode",
    "price",
    "currency",
  ]);
  const featureCode = useWatch({ control: form.control, name: "featureCode" });
  const chosenFeature = catalog.features.find((feature) => feature.code === featureCode);
  const replacesCurrentAddOn = brand.addOns.some((addOn) => addOn.featureCode === featureCode);
  const giveAddOn = useMutation({
    mutationFn: (values: GiveAddOnFormValues) =>
      giveBrandAddOn(brand.id, toGiveAddOnRequest(values)),
  });
  const brandRoute = backofficeRoutes.brand(brand.id);

  const chooseFeature = (code: string) => {
    form.setValue("featureCode", code);
    form.setValue("limit", "");
    const feature = catalog.features.find((candidate) => candidate.code === code);
    if (feature?.addOnListPrice != null && feature.currency !== null) {
      form.setValue("price", amountText(feature.addOnListPrice), { shouldValidate: true });
      form.setValue("currency", feature.currency, { shouldValidate: true });
    }
  };

  const save = form.handleSubmit(async (values) => {
    setSubmissionMessage(null);
    try {
      await giveAddOn.mutateAsync(values);
      await queryClient.invalidateQueries({ queryKey: brandsQueryKey });
      showToast(translate("backoffice.giveAddOn.saved"));
      router.replace(brandRoute);
    } catch (error) {
      setSubmissionMessage(applySubmissionError(form, error, formFieldNames));
    }
  });

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          eyebrow={brand.name}
          title={translate("backoffice.giveAddOn.title")}
          navigation="back"
          onNavigate={() => router.replace(brandRoute)}
        />
      }
    >
      <RequiredFieldsLegend />
      <View className="gap-2">
        <AppText variant="bodyStrong">
          {withRequiredMark(translate("backoffice.giveAddOn.feature"))}
        </AppText>
        <ChipGroup
          options={catalog.features.map((feature) => ({
            value: feature.code,
            label: featureLabel(feature.code),
          }))}
          selectedValue={featureCode}
          onChange={chooseFeature}
        />
      </View>
      {replacesCurrentAddOn ? (
        <Banner tone="info" message={translate("backoffice.giveAddOn.replaces")} />
      ) : null}
      {chosenFeature?.isCounted ? (
        <Controller
          control={form.control}
          name="limit"
          render={({ field, fieldState }) => (
            <TextField
              label={translate("backoffice.giveAddOn.limit")}
              hint={translate("backoffice.giveAddOn.limitHint")}
              keyboardType="number-pad"
              value={field.value}
              onChangeText={field.onChange}
              onBlur={field.onBlur}
              errorMessage={fieldState.error?.message}
            />
          )}
        />
      ) : null}
      <SubscriptionTermsFields control={form.control} />
      {submissionMessage === null ? null : <Banner message={submissionMessage} />}
      <Button
        label={translate("backoffice.giveAddOn.submit")}
        onPress={save}
        disabled={!areRequiredFieldsFilled}
        isLoading={giveAddOn.isPending}
      />
    </ScrollScreen>
  );
}

export function GiveAddOnScreen({ brandId }: GiveAddOnScreenProps) {
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
        header={<ScreenHeader title={translate("backoffice.giveAddOn.title")} navigation="back" />}
      >
        <Banner message={translate("common.unexpectedError")} />
      </ScrollScreen>
    );
  }
  if (brand.data === undefined || catalog.data === undefined) {
    return <SkeletonScreen />;
  }
  return <GiveAddOnForm brand={brand.data} catalog={catalog.data} />;
}
