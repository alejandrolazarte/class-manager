import { useState } from "react";
import { View } from "react-native";
import { variantLabel } from "@/features/products/stockLabels";
import { Product, StockLoadKind, stockLoadKinds } from "@/features/products/types";
import { useRecordStockMovement } from "@/features/products/useProductMutations";
import { translate, TranslationKey } from "@/i18n/translate";
import { isFilled } from "@/forms/requiredFields";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { BottomSheet } from "@/ui/BottomSheet";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { SegmentedControl } from "@/ui/SegmentedControl";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

interface StockMovementSheetProps {
  product: Product;
  initialKind: StockLoadKind;
  onClose: () => void;
}

const signedWholeNumberPattern = /^-?\d+$/;

export function StockMovementSheet({ product, initialKind, onClose }: StockMovementSheetProps) {
  const { showToast } = useToast();
  const recordStockMovementMutation = useRecordStockMovement(product.id);
  const [variantId, setVariantId] = useState(product.variants[0]?.id ?? "");
  const [kind, setKind] = useState<StockLoadKind>(initialKind);
  const [quantityText, setQuantityText] = useState("");
  const [note, setNote] = useState("");
  const [hasFailed, setHasFailed] = useState(false);
  const isRestock = kind === "Restock";

  const submit = async () => {
    setHasFailed(false);
    const trimmedQuantity = quantityText.trim();
    if (!signedWholeNumberPattern.test(trimmedQuantity)) {
      setHasFailed(true);
      return;
    }
    try {
      await recordStockMovementMutation.mutateAsync({
        variantId,
        kind,
        quantity: Number(trimmedQuantity),
        note: note.trim().length === 0 ? null : note.trim(),
      });
      showToast(translate("products.stock.saved"));
      onClose();
    } catch {
      setHasFailed(true);
    }
  };

  return (
    <BottomSheet onClose={onClose}>
      <AppText variant="headline">
        {translate(isRestock ? "products.stock.load" : "products.stock.adjust")}
      </AppText>
      <SegmentedControl
        options={stockLoadKinds.map((loadKind) => ({
          value: loadKind,
          label: translate(`products.stock.kind.${loadKind}` as TranslationKey),
        }))}
        selectedValue={kind}
        onChange={setKind}
      />
      {product.variants.length > 1 ? (
        <View className="flex-row flex-wrap gap-2">
          {product.variants.map((variant) => (
            <Chip
              key={variant.id}
              label={`${variantLabel(variant)} · ${variant.stock ?? 0}`}
              accessibilityLabel={variantLabel(variant)}
              isSelected={variantId === variant.id}
              onPress={() => setVariantId(variant.id)}
            />
          ))}
        </View>
      ) : null}
      <TextField
        label={translate(isRestock ? "products.stock.quantity" : "products.stock.adjustment")}
        isRequired
        keyboardType={isRestock ? "number-pad" : "default"}
        value={quantityText}
        onChangeText={setQuantityText}
      />
      <TextField
        label={translate(isRestock ? "products.stock.noteOptional" : "products.stock.note")}
        isRequired={!isRestock}
        value={note}
        onChangeText={setNote}
      />
      {hasFailed ? <Banner tone="warning" message={translate("products.stock.invalid")} /> : null}
      <Button
        label={translate(isRestock ? "products.stock.load" : "products.stock.adjust")}
        onPress={submit}
        disabled={!isFilled(quantityText) || (!isRestock && !isFilled(note))}
        isLoading={recordStockMovementMutation.isPending}
      />
    </BottomSheet>
  );
}
