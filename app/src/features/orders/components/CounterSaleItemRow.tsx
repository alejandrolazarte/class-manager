import { View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { CatalogImage } from "@/features/catalogImages/components/CatalogImage";
import { formatMoney } from "@/features/fees/money";
import { CounterSaleItem } from "@/features/orders/counterSale";
import { QuantityStepper } from "@/features/studentApp/components/QuantityStepper";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { IconButton } from "@/ui/IconButton";

interface CounterSaleItemRowProps {
  item: CounterSaleItem;
  units: number;
  onChangeUnits: (units: number) => void;
  isLocked?: boolean;
  showsLineTotal?: boolean;
}

export function CounterSaleItemRow({
  item,
  units,
  onChangeUnits,
  isLocked = false,
  showsLineTotal = false,
}: CounterSaleItemRowProps) {
  const currencyCode = useBusinessCurrency();
  const price = formatMoney(item.price, currencyCode);
  const isUnavailable = isLocked || item.maximumUnits === 0;
  const stepper = (
    <View className="self-start rounded-xl border-[1.5px] border-border">
      <QuantityStepper
        units={units}
        minimum={0}
        maximum={item.maximumUnits}
        onChange={onChangeUnits}
        decreaseIcon={units <= 1 ? "delete" : "remove"}
        decreaseLabel={translate("orders.counterSale.decrease", { name: item.name })}
        increaseLabel={translate("orders.counterSale.increase", { name: item.name })}
      />
    </View>
  );
  const trailing =
    units === 0 ? (
      <IconButton
        icon="add"
        variant="outlined"
        tone="primary"
        accessibilityLabel={translate("orders.counterSale.add", { name: item.name })}
        disabled={isUnavailable}
        onPress={() => onChangeUnits(1)}
      />
    ) : showsLineTotal ? null : (
      stepper
    );
  return (
    <View
      className={`flex-row items-center gap-3 rounded-[20px] border-[1.5px] border-border bg-surface p-2.5 ${isUnavailable && units === 0 ? "opacity-50" : ""}`}
    >
      <CatalogImage
        imageUri={item.imageUrl}
        placeholderIcon={item.kind === "pack" ? "classPacks" : "products"}
        placeholderIconSize="large"
        className={`${showsLineTotal ? "h-[76px] w-[76px]" : "h-14 w-14"} self-start rounded-[14px]`}
      />
      <View className="min-w-0 flex-1 gap-1.5">
        <View className="flex-row items-start justify-between gap-2">
          <View className="min-w-0 flex-1 gap-0.5">
            <AppText variant="bodyStrong">{item.name}</AppText>
            <AppText variant="caption" tone="subtle">
              {[price, item.detail].filter(Boolean).join(" · ")}
            </AppText>
          </View>
          {showsLineTotal ? (
            <AppText variant="bodyStrong" className="font-heavy">
              {formatMoney(item.price * units, currencyCode)}
            </AppText>
          ) : null}
        </View>
        {showsLineTotal && units > 0 ? stepper : null}
      </View>
      {trailing}
    </View>
  );
}
