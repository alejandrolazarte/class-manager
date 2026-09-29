import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { StockPanel } from "@/features/products/components/StockPanel";
import { productErrorCodes } from "@/features/products/productErrorCodes";
import {
  productFieldNames,
  ProductFormValues,
  productSchema,
  toProductFormValues,
  toSaveProductRequest,
} from "@/features/products/productSchema";
import { Product, stockModes } from "@/features/products/types";
import { useSaveProduct, useSetProductActive } from "@/features/products/useProductMutations";
import { useProducts } from "@/features/products/useProducts";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { TextField } from "@/ui/TextField";
import { ToggleSwitch } from "@/ui/ToggleSwitch";
import { useToast } from "@/ui/ToastProvider";

const badRequestStatus = 400;

interface ProductFormScreenProps {
  productId?: string;
}

interface ProductEditorProps {
  product?: Product;
}

interface ProductFieldOptions {
  name: "name" | "description" | "price" | "variantNames";
  labelKey: TranslationKey;
  placeholderKey?: TranslationKey;
  keyboardType?: "default" | "decimal-pad";
}

function ProductEditor({ product }: ProductEditorProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const saveProductMutation = useSaveProduct();
  const setProductActiveMutation = useSetProductActive();
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const form = useForm<ProductFormValues>({
    resolver: zodResolver(productSchema),
    defaultValues: toProductFormValues(product),
    mode: "onTouched",
  });

  const save = form.handleSubmit(async (formValues) => {
    setSubmissionFailure(null);
    try {
      await saveProductMutation.mutateAsync({
        productId: product?.id,
        request: toSaveProductRequest(formValues, product),
      });
      showToast(translate("products.form.saved"));
      router.back();
    } catch (saveError) {
      if (isApiError(saveError) && saveError.hasCode(productErrorCodes.nameTaken)) {
        form.setError("name", {
          type: "server",
          message: translate("products.validation.nameTaken"),
        });
        return;
      }
      const hasFieldErrors =
        isApiError(saveError) &&
        saveError.status === badRequestStatus &&
        applyServerFieldErrors(form, saveError.problem, productFieldNames);
      if (!hasFieldErrors) {
        setSubmissionFailure(toSubmissionFailure(saveError));
      }
    }
  });

  const toggleActive = async () => {
    if (product === undefined) {
      return;
    }
    setSubmissionFailure(null);
    const isActive = !product.isActive;
    try {
      await setProductActiveMutation.mutateAsync({ productId: product.id, isActive });
      showToast(translate(isActive ? "products.form.activated" : "products.form.deactivated"));
      router.back();
    } catch (activationError) {
      setSubmissionFailure(toSubmissionFailure(activationError));
    }
  };

  const renderField = ({ name, labelKey, placeholderKey, keyboardType }: ProductFieldOptions) => (
    <Controller
      control={form.control}
      name={name}
      render={({ field, fieldState }) => (
        <TextField
          label={translate(labelKey)}
          placeholder={placeholderKey ? translate(placeholderKey) : undefined}
          keyboardType={keyboardType}
          value={field.value}
          onChangeText={field.onChange}
          onBlur={field.onBlur}
          errorMessage={fieldState.error?.message}
        />
      )}
    />
  );

  return (
    <SettingsFormScreenLayout
      title={translate(product ? "products.form.editTitle" : "products.form.newTitle")}
      submissionFailure={submissionFailure}
      onRetry={save}
    >
      {renderField({
        name: "name",
        labelKey: "products.form.name",
        placeholderKey: "products.form.namePlaceholder",
      })}
      {renderField({ name: "price", labelKey: "products.form.price", keyboardType: "decimal-pad" })}
      {renderField({
        name: "variantNames",
        labelKey: "products.form.variants",
        placeholderKey: "products.form.variantsPlaceholder",
      })}
      {renderField({ name: "description", labelKey: "products.form.description" })}
      <Controller
        control={form.control}
        name="stockMode"
        render={({ field }) => (
          <View className="gap-2">
            <AppText variant="label" tone="muted">
              {translate("products.form.stockMode")}
            </AppText>
            <View className="flex-row flex-wrap gap-2">
              {stockModes.map((stockMode) => (
                <Chip
                  key={stockMode}
                  label={translate(`products.stockMode.${stockMode}` as TranslationKey)}
                  isSelected={field.value === stockMode}
                  onPress={() => field.onChange(stockMode)}
                />
              ))}
            </View>
            <AppText variant="caption" tone="subtle">
              {translate(`products.stockModeHint.${field.value}` as TranslationKey)}
            </AppText>
          </View>
        )}
      />
      <Controller
        control={form.control}
        name="isVisibleInApp"
        render={({ field }) => (
          <ToggleSwitch
            label={translate("products.form.visibleInApp")}
            value={field.value}
            onValueChange={field.onChange}
          />
        )}
      />
      <View className="gap-3">
        <Button
          label={translate("common.save")}
          onPress={save}
          isLoading={saveProductMutation.isPending}
        />
        {product ? (
          <Button
            variant={product.isActive ? "dangerOutline" : "outline"}
            size="medium"
            label={translate(
              product.isActive ? "products.form.deactivate" : "products.form.activate",
            )}
            onPress={toggleActive}
            isLoading={setProductActiveMutation.isPending}
          />
        ) : null}
      </View>
      {product && product.stockMode !== "Unlimited" ? <StockPanel product={product} /> : null}
    </SettingsFormScreenLayout>
  );
}

export function ProductFormScreen({ productId }: ProductFormScreenProps) {
  const productsQuery = useProducts(true, { enabled: productId !== undefined });
  if (productId === undefined) {
    return <ProductEditor />;
  }
  const product = productsQuery.data?.find((candidate) => candidate.id === productId);
  if (product === undefined) {
    return (
      <SettingsItemState
        isPending={productsQuery.isPending}
        isError={productsQuery.isError}
        notFoundMessage={translate("products.form.notFound")}
        onRetry={() => productsQuery.refetch()}
      />
    );
  }
  return <ProductEditor key={product.id} product={product} />;
}
