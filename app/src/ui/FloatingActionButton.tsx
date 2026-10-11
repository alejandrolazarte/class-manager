import { Pressable } from "react-native";
import { AppText } from "@/ui/AppText";
import { useFloatingShadowStyle } from "@/ui/elevation";
import { Icon } from "@/ui/Icon";

interface FloatingActionButtonProps {
  label: string;
  onPress: () => void;
}

export function FloatingActionButton({ label, onPress }: FloatingActionButtonProps) {
  const elevationStyle = useFloatingShadowStyle();
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={label}
      onPress={onPress}
      style={elevationStyle}
      className="absolute bottom-5 right-[18px] h-12 flex-row items-center gap-1.5 rounded-full bg-primary pl-3.5 pr-[18px] active:bg-primary-strong"
    >
      <Icon name="add" size="medium" tone="primary-foreground" />
      <AppText variant="bodyStrong" tone="onPrimary">
        {label}
      </AppText>
    </Pressable>
  );
}
