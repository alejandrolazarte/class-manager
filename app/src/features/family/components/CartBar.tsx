import { Pressable, View } from "react-native";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { useElevationStyle } from "@/ui/elevation";

interface CartBarProps {
  units: number;
  totalLabel: string;
  onPress: () => void;
}

export function CartBar({ units, totalLabel, onPress }: CartBarProps) {
  const floatingStyle = useElevationStyle("floating");
  const label = translate("family.shop.viewCart");
  return (
    <View pointerEvents="box-none" className="absolute bottom-3 left-4 right-4 items-center">
      <Pressable
        accessibilityRole="button"
        accessibilityLabel={`${label}, ${units}, ${totalLabel}`}
        onPress={onPress}
        style={floatingStyle}
        className="h-14 w-full max-w-2xl flex-row items-center gap-2.5 rounded-[18px] bg-primary px-4 active:bg-primary-strong"
      >
        <View className="h-[30px] min-w-[30px] items-center justify-center rounded-full bg-primary-foreground/20 px-1">
          <AppText variant="link" tone="onPrimary" className="font-heavy">
            {units}
          </AppText>
        </View>
        <AppText variant="button" tone="onPrimary" className="flex-1">
          {label}
        </AppText>
        <AppText variant="heading" tone="onPrimary" className="font-heavy">
          {totalLabel}
        </AppText>
      </Pressable>
    </View>
  );
}
