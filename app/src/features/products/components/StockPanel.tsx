import { useState } from "react";
import { View } from "react-native";
import { variantLabel } from "@/features/products/stockLabels";
import { Product, StockMovementKind } from "@/features/products/types";
import { useRecordStockMovement } from "@/features/products/useProductMutations";
import { useStockMovements } from "@/features/products/useProducts";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Chip } from "@/ui/Chip";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

interface StockPanelProps {
  product: Product;
}

type LoadKind = Extract<StockMovementKind, "Restock" | "Adjustment">;

const loadKinds: readonly LoadKind[] = ["Restock", "Adjustment"];
const signedWholeNumberPattern = /^-?\d+$/;
const recentMovementCount = 10;

export function StockPanel({ product }: StockPanelProps) {
  const { showToast } = useToast();
  const recordStockMovementMutation = useRecordStockMovement(product.id);
  const { data: movements = [] } = useStockMovements(product.id);
  const [variantId, setVariantId] = useState(product.variants[0]?.id ?? "");
  const [kind, setKind] = useState<LoadKind>("Restock");
  const [quantityText, setQuantityText] = useState("");
  const [note, setNote] = useState("");
  const [hasFailed, setHasFailed] = useState(false);
  const variantsById = new Map(product.variants.map((variant) => [variant.id, variant]));

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
      setQuantityText("");
      setNote("");
      showToast(translate("products.stock.saved"));
    } catch {
      setHasFailed(true);
    }
  };

  return (
    <Card className="gap-3 p-4">
      <AppText variant="bodyStrong">{translate("products.stock.title")}</AppText>
      {product.variants.map((variant) => (
        <View key={variant.id} className="flex-row justify-between">
          <AppText variant="body">{variantLabel(variant)}</AppText>
          <AppText variant="bodyStrong">{variant.stock ?? 0}</AppText>
        </View>
      ))}
      {product.variants.length > 1 ? (
        <View className="flex-row flex-wrap gap-2">
          {product.variants.map((variant) => (
            <Chip
              key={variant.id}
              label={variantLabel(variant)}
              isSelected={variantId === variant.id}
              onPress={() => setVariantId(variant.id)}
            />
          ))}
        </View>
      ) : null}
      <View className="flex-row flex-wrap gap-2">
        {loadKinds.map((loadKind) => (
          <Chip
            key={loadKind}
            label={translate(`products.stock.kind.${loadKind}` as TranslationKey)}
            isSelected={kind === loadKind}
            onPress={() => setKind(loadKind)}
          />
        ))}
      </View>
      <TextField
        label={translate(
          kind === "Restock" ? "products.stock.quantity" : "products.stock.adjustment",
        )}
        keyboardType={kind === "Restock" ? "number-pad" : "default"}
        value={quantityText}
        onChangeText={setQuantityText}
      />
      <TextField
        label={translate(
          kind === "Restock" ? "products.stock.noteOptional" : "products.stock.note",
        )}
        value={note}
        onChangeText={setNote}
      />
      {hasFailed ? <Banner tone="warning" message={translate("products.stock.invalid")} /> : null}
      <Button
        size="medium"
        label={translate(kind === "Restock" ? "products.stock.load" : "products.stock.adjust")}
        onPress={submit}
        isLoading={recordStockMovementMutation.isPending}
      />
      {movements.length > 0 ? (
        <View className="gap-1.5">
          <AppText variant="label" tone="muted">
            {translate("products.stock.history")}
          </AppText>
          {movements.slice(0, recentMovementCount).map((movement) => {
            const variant = variantsById.get(movement.variantId);
            return (
              <AppText key={movement.id} variant="caption" tone="subtle">
                {[
                  translate(`products.stock.kind.${movement.kind}` as TranslationKey),
                  `${movement.quantity > 0 ? "+" : ""}${movement.quantity}`,
                  variant && variant.name.length > 0 ? variant.name : null,
                  movement.note,
                ]
                  .filter(Boolean)
                  .join(" · ")}
              </AppText>
            );
          })}
        </View>
      ) : null}
    </Card>
  );
}
