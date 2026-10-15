import { View } from "react-native";
import { AppText } from "@/ui/AppText";
import { useFloatingShadowStyle } from "@/ui/elevation";
import { Entrance } from "@/ui/motion/Entrance";
import { PressableScale } from "@/ui/motion/PressableScale";

interface FloatingBarButtonProps {
  label: string;
  count: number;
  value: string;
  accessibilityLabel: string;
  onPress: () => void;
}

export function FloatingBarButton({
  label,
  count,
  value,
  accessibilityLabel,
  onPress,
}: FloatingBarButtonProps) {
  const floatingStyle = useFloatingShadowStyle();
  return (
    <View pointerEvents="box-none" className="absolute bottom-3 left-4 right-4 items-center">
      <Entrance kind="springRise" className="w-full max-w-2xl">
        <PressableScale
          accessibilityRole="button"
          accessibilityLabel={accessibilityLabel}
          onPress={onPress}
          style={floatingStyle}
          className="h-[52px] w-full flex-row items-center gap-2.5 rounded-full bg-primary pl-2.5 pr-4 active:bg-primary-strong"
        >
          <View className="h-8 min-w-8 items-center justify-center rounded-full bg-primary-foreground/20 px-1.5">
            <AppText variant="bodyStrong" tone="onPrimary" className="font-heavy">
              {count}
            </AppText>
          </View>
          <AppText variant="button" tone="onPrimary" className="flex-1">
            {label}
          </AppText>
          <AppText variant="button" tone="onPrimary">
            {value}
          </AppText>
        </PressableScale>
      </Entrance>
    </View>
  );
}
