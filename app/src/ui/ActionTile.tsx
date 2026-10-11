import { AppText } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";
import { PressableScale } from "@/ui/motion/PressableScale";

interface ActionTileProps {
  icon: IconName;
  label: string;
  onPress: () => void;
}

export function ActionTile({ icon, label, onPress }: ActionTileProps) {
  return (
    <PressableScale
      accessibilityRole="button"
      accessibilityLabel={label}
      onPress={onPress}
      className="min-h-[54px] flex-1 items-center justify-center gap-1 rounded-xl bg-muted px-2 py-2 active:opacity-80"
    >
      <Icon name={icon} size="medium" tone="primary" />
      <AppText variant="badge" numberOfLines={1}>
        {label}
      </AppText>
    </PressableScale>
  );
}
