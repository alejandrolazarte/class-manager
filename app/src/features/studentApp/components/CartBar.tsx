import { View } from "react-native";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { useFloatingShadowStyle } from "@/ui/elevation";
import { Entrance } from "@/ui/motion/Entrance";
import { PressableScale } from "@/ui/motion/PressableScale";

interface CartBarProps {
  units: number;
  totalLabel: string;
  onPress: () => void;
}

export function CartBar({ units, totalLabel, onPress }: CartBarProps) {
  const floatingStyle = useFloatingShadowStyle();
  const label = translate("student.shop.viewCart");
  return (
    <View pointerEvents="box-none" className="absolute bottom-3 left-4 right-4 items-center">
      <Entrance kind="springRise" className="w-full max-w-2xl">
        <PressableScale
          accessibilityRole="button"
          accessibilityLabel={`${label}, ${units}, ${totalLabel}`}
          onPress={onPress}
          style={floatingStyle}
          className="h-[52px] w-full flex-row items-center gap-2.5 rounded-full bg-primary pl-2.5 pr-4 active:bg-primary-strong"
        >
          <View className="h-8 min-w-8 items-center justify-center rounded-full bg-primary-foreground/20 px-1.5">
            <AppText variant="bodyStrong" tone="onPrimary" className="font-heavy">
              {units}
            </AppText>
          </View>
          <AppText variant="button" tone="onPrimary" className="flex-1">
            {label}
          </AppText>
          <AppText variant="button" tone="onPrimary">
            {totalLabel}
          </AppText>
        </PressableScale>
      </Entrance>
    </View>
  );
}
