import { ThemeColorToken } from "@/theme/themeColorTokens";
import { AppText, TextTone } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";
import { PressableScale } from "@/ui/motion/PressableScale";

type TonalButtonTone = "primary" | "success";

interface TonalButtonProps {
  label: string;
  icon: IconName;
  onPress: () => void;
  tone?: TonalButtonTone;
  isHidden?: boolean;
  accessibilityLabel?: string;
}

const toneClassNames: Record<TonalButtonTone, string> = {
  primary: "bg-primary-soft",
  success: "bg-success-soft",
};

const iconTones: Record<TonalButtonTone, ThemeColorToken> = {
  primary: "primary-soft-foreground",
  success: "success-soft-foreground",
};

const textTones: Record<TonalButtonTone, TextTone> = {
  primary: "primarySoft",
  success: "successSoft",
};

export function TonalButton({
  label,
  icon,
  onPress,
  tone = "primary",
  isHidden = false,
  accessibilityLabel,
}: TonalButtonProps) {
  return (
    <PressableScale
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel ?? label}
      accessibilityElementsHidden={isHidden}
      importantForAccessibility={isHidden ? "no-hide-descendants" : "auto"}
      disabled={isHidden}
      onPress={onPress}
      className={`h-8 flex-row items-center gap-1 rounded-full px-3 active:opacity-80 ${toneClassNames[tone]} ${isHidden ? "opacity-0" : ""}`}
    >
      <Icon name={icon} size="small" tone={iconTones[tone]} />
      <AppText variant="link" tone={textTones[tone]}>
        {label}
      </AppText>
    </PressableScale>
  );
}
