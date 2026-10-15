import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm, useWatch } from "react-hook-form";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { catalogImageSaveMessageKey } from "@/features/catalogImages/catalogImageSaveMessage";
import { CatalogImageField } from "@/features/catalogImages/components/CatalogImageField";
import { useCatalogImageDraft } from "@/features/catalogImages/useCatalogImageDraft";
import { InitialStockFields } from "@/features/products/components/InitialStockFields";
import { ProductColorsField } from "@/features/products/components/ProductColorsField";
import { ProductStockSection } from "@/features/products/components/ProductStockSection";
import { productErrorCodes } from "@/features/products/productErrorCodes";
import {
  productFieldNames,
  ProductFormValues,
  productLimits,
  productSchema,
  toProductFormValues,
  toSaveProductRequest,
} from "@/features/products/productSchema";
import { Product, stockModes } from "@/features/products/types";
import {
  useApplyProductImages,
  useSaveProduct,
  useSetProductActive,
} from "@/features/products/useProductMutations";
import { useProducts } from "@/features/products/useProducts";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { TagInput } from "@/ui/TagInput";
import { TextField } from "@/ui/TextField";
import { ToggleSwitch } from "@/ui/ToggleSwitch";
import { useToast } from "@/ui/ToastProvider";
import { useRequiredFieldsFilled } from "@/forms/requiredFields";
import { RequiredFieldsLegend, withRequiredMark } from "@/ui/RequiredFieldsLegend";

const badRequestStatus = 400;
const letterSizes = ["XS", "S", "M", "L", "XL", "XXL"];
const firstNumberSize = 35;
const lastNumberSize = 45;
const numberSizes = Array.from({ length: lastNumberSize - firstNumberSize + 1 }, (_, index) =>
  String(firstNumberSize + index),
);
const sizeSuggestions = [...letterSizes, ...numberSizes];
const sizePresets = [
  { label: translate("products.form.variantsLetters"), values: letterSizes },
  { label: translate("products.form.variantsNumbers"), values: numberSizes },
];

interface ProductFormScreenProps {
  productId?: string;
}

interface ProductEditorProps {
  product?: Product;
}

interface ProductFieldOptions {
  name: "name" | "description" | "price";
  labelKey: TranslationKey;
  placeholderKey?: TranslationKey;
  isMultiline?: boolean;
  isRequired?: boolean;
  keyboardType?: "default" | "decimal-pad";
}

function ProductEditor({ product }: ProductEditorProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const saveProductMutation = useSaveProduct();
  const setProductActiveMutation = useSetProductActive();
  const applyProductImagesMutation = useApplyProductImages();
  const images = useCatalogImageDraft(product?.images ?? []);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const form = useForm<ProductFormValues>({
    resolver: zodResolver(productSchema),
    defaultValues: toProductFormValues(product),
    mode: "onTouched",
  });
  const areRequiredFieldsFilled = useRequiredFieldsFilled(form.control, ["name", "price"]);
  const stockMode = useWatch({ control: form.control, name: "stockMode" });
  const sizes = useWatch({ control: form.control, name: "sizes" });
  const colors = useWatch({ control: form.control, name: "colors" });

  const save = form.handleSubmit(async (formValues) => {
    setSubmissionFailure(null);
    try {
      const savedProduct = await saveProductMutation.mutateAsync({
        productId: product?.id,
        request: toSaveProductRequest(formValues, product),
      });
      const imagesOutcome = await applyProductImagesMutation.mutateAsync({
        productId: savedProduct.id,
        draft: images.draft,
        savedImages: savedProduct.images,
      });
      showToast(translate(catalogImageSaveMessageKey(imagesOutcome, "products.form.saved")));
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

  const renderField = ({
    name,
    labelKey,
    placeholderKey,
    isMultiline = false,
    isRequired = false,
    keyboardType,
  }: ProductFieldOptions) => (
    <Controller
      control={form.control}
      name={name}
      render={({ field, fieldState }) => (
        <TextField
          label={translate(labelKey)}
          isRequired={isRequired}
          placeholder={placeholderKey ? translate(placeholderKey) : undefined}
          keyboardType={keyboardType}
          multiline={isMultiline}
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
      <RequiredFieldsLegend />
      {renderField({
        name: "name",
        labelKey: "products.form.name",
        placeholderKey: "products.form.namePlaceholder",
        isRequired: true,
      })}
      {renderField({
        name: "price",
        labelKey: "products.form.price",
        keyboardType: "decimal-pad",
        isRequired: true,
      })}
      <Controller
        control={form.control}
        name="sizes"
        render={({ field, fieldState }) => (
          <TagInput
            label={translate("products.form.sizes")}
            values={field.value}
            onChange={field.onChange}
            addLabel={translate("products.form.variantsAdd")}
            removeLabelOf={(name) => translate("products.form.variantRemove", { name })}
            duplicateMessageOf={(name) => translate("products.form.variantRepeated", { name })}
            placeholder={translate("products.form.variantsPlaceholder")}
            suggestions={sizeSuggestions}
            presets={sizePresets}
            maximumLength={productLimits.sizeMaximumLength}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <Controller
        control={form.control}
        name="colors"
        render={({ field }) => (
          <ProductColorsField colors={field.value} onChange={field.onChange} />
        )}
      />
      {renderField({
        name: "description",
        labelKey: "products.form.description",
        isMultiline: true,
      })}
      <CatalogImageField
        images={images}
        placeholderIcon="products"
        maximumImageCount={productLimits.maximumImageCount}
      />
      <Controller
        control={form.control}
        name="stockMode"
        render={({ field }) => (
          <View className="gap-2">
            <AppText variant="label" tone="muted">
              {withRequiredMark(translate("products.form.stockMode"))}
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
      {product === undefined && stockMode !== "Unlimited" ? (
        <Controller
          control={form.control}
          name="initialStock"
          render={({ field }) => (
            <InitialStockFields
              sizes={sizes}
              colors={colors}
              initialStock={field.value}
              onChange={field.onChange}
            />
          )}
        />
      ) : null}
      {product && product.stockMode !== "Unlimited" ? (
        <ProductStockSection product={product} />
      ) : null}
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
          disabled={!areRequiredFieldsFilled}
          isLoading={saveProductMutation.isPending || applyProductImagesMutation.isPending}
        />
        {product ? (
          <Button
            variant={product.isActive ? "dangerOutline" : "secondary"}
            size="medium"
            label={translate(
              product.isActive ? "products.form.deactivate" : "products.form.activate",
            )}
            onPress={toggleActive}
            isLoading={setProductActiveMutation.isPending}
          />
        ) : null}
      </View>
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
        navigation="close"
        isPending={productsQuery.isPending}
        isError={productsQuery.isError}
        notFoundMessage={translate("products.form.notFound")}
        onRetry={() => productsQuery.refetch()}
      />
    );
  }
  return <ProductEditor key={product.id} product={product} />;
}
