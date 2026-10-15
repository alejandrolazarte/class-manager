import { translate } from "@/i18n/translate";
import { FloatingBarButton } from "@/ui/FloatingBarButton";

interface CartBarProps {
  units: number;
  totalLabel: string;
  onPress: () => void;
}

export function CartBar({ units, totalLabel, onPress }: CartBarProps) {
  const label = translate("student.shop.viewCart");
  return (
    <FloatingBarButton
      label={label}
      count={units}
      value={totalLabel}
      accessibilityLabel={`${label}, ${units}, ${totalLabel}`}
      onPress={onPress}
    />
  );
}
