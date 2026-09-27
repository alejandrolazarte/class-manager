import { Pressable } from "react-native";
import { AppText } from "@/ui/AppText";
import { useElevationStyle } from "@/ui/elevation";
import { Icon } from "@/ui/Icon";

interface FloatingActionButtonProps {
  label: string;
  onPress: () => void;
}

export function FloatingActionButton({ label, onPress }: FloatingActionButtonProps) {
  const elevationStyle = useElevationStyle("floating");
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={label}
      onPress={onPress}
      style={elevationStyle}
      className="absolute bottom-5 right-[18px] h-14 flex-row items-center gap-2 rounded-[18px] bg-primary pl-4 pr-5 active:bg-primary-strong"
    >
      <Icon name="add" size="extraLarge" tone="primary-foreground" />
      <AppText variant="bodyStrong" tone="onPrimary">
        {label}
      </AppText>
    </Pressable>
  );
}
