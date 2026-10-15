import { AccessibilityRole, View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";
import { PressableScale } from "@/ui/motion/PressableScale";

type TextLinkAlignment = "start" | "center";

interface TextLinkProps {
  label: string;
  onPress: () => void;
  accessibilityLabel?: string;
  accessibilityRole?: AccessibilityRole;
  icon?: IconName;
  trailingIcon?: IconName;
  isExpanded?: boolean;
  alignment?: TextLinkAlignment;
}

const alignmentClassNames: Record<TextLinkAlignment, string> = {
  start: "self-start",
  center: "self-center",
};

const textAlignmentClassNames: Record<TextLinkAlignment, string> = {
  start: "",
  center: "text-center",
};

export function TextLink({
  label,
  onPress,
  accessibilityLabel,
  accessibilityRole = "button",
  icon,
  trailingIcon,
  isExpanded,
  alignment = "start",
}: TextLinkProps) {
  return (
    <PressableScale
      accessibilityRole={accessibilityRole}
      accessibilityLabel={accessibilityLabel ?? label}
      accessibilityState={isExpanded === undefined ? undefined : { expanded: isExpanded }}
      onPress={onPress}
      hitSlop={8}
      className={`flex-row items-center gap-0.5 py-1.5 active:opacity-70 ${alignmentClassNames[alignment]}`}
    >
      {icon === undefined ? null : (
        <View className="mr-0.5">
          <Icon name={icon} size="small" tone="primary" />
        </View>
      )}
      <AppText
        variant="link"
        tone="primary"
        className={`flex-shrink ${textAlignmentClassNames[alignment]}`}
      >
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
