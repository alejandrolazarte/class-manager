import { AccessibilityRole, View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";
import { PressableScale } from "@/ui/motion/PressableScale";

interface TextLinkProps {
  label: string;
  onPress: () => void;
  accessibilityLabel?: string;
  accessibilityRole?: AccessibilityRole;
  trailingIcon?: IconName;
}

export function TextLink({
  label,
  onPress,
  accessibilityLabel,
  accessibilityRole = "button",
  trailingIcon,
}: TextLinkProps) {
  return (
    <PressableScale
      accessibilityRole={accessibilityRole}
      accessibilityLabel={accessibilityLabel ?? label}
      onPress={onPress}
      hitSlop={8}
      className="flex-row items-center gap-0.5 self-start py-1.5 active:opacity-70"
    >
      <AppText variant="link" tone="primary">
        {label}
      </AppText>
      {trailingIcon === undefined ? null : (
        <View>
          <Icon name={trailingIcon} size="small" tone="primary" />
        </View>
      )}
    </PressableScale>
  );
}
