import { Avatar } from "@/ui/Avatar";
import { PressableScale } from "@/ui/motion/PressableScale";

interface AvatarButtonProps {
  name: string;
  photoUrl?: string | null;
  accessibilityLabel: string;
  onPress: () => void;
}

export function AvatarButton({ name, photoUrl, accessibilityLabel, onPress }: AvatarButtonProps) {
  return (
    <PressableScale
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel}
      onPress={onPress}
      hitSlop={4}
      className="h-11 w-11 items-center justify-center rounded-full"
    >
      <Avatar name={name} size="small" photoUrl={photoUrl} />
    </PressableScale>
  );
}
