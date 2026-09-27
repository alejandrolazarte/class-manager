import { View } from "react-native";
import { AppText, TextTone, TextVariant } from "@/ui/AppText";

export type AvatarTone = "primarySoft" | "primary" | "muted" | "success" | "danger";

type AvatarSize = "small" | "medium" | "large";

interface AvatarProps {
  name: string;
  tone?: AvatarTone;
  size?: AvatarSize;
}

const maximumInitials = 2;
const nameSeparatorPattern = /\s+/;

const toneClassNames: Record<AvatarTone, string> = {
  primarySoft: "bg-primary-soft",
  primary: "bg-primary",
  muted: "bg-muted",
  success: "bg-success-soft",
  danger: "bg-danger-soft",
};

const textTones: Record<AvatarTone, TextTone> = {
  primarySoft: "primarySoft",
  primary: "onPrimary",
  muted: "muted",
  success: "successSoft",
  danger: "dangerSoft",
};

const sizeClassNames: Record<AvatarSize, string> = {
  small: "h-10 w-10",
  medium: "h-11 w-11",
  large: "h-[60px] w-[60px]",
};

const textVariants: Record<AvatarSize, TextVariant> = {
  small: "link",
  medium: "bodyStrong",
  large: "headline",
};

export function initialsOf(name: string): string {
  return name
    .trim()
    .split(nameSeparatorPattern)
    .filter(Boolean)
    .slice(0, maximumInitials)
    .map((namePart) => namePart.charAt(0).toUpperCase())
    .join("");
}

export function Avatar({ name, tone = "primarySoft", size = "medium" }: AvatarProps) {
  return (
    <View
      importantForAccessibility="no-hide-descendants"
      accessibilityElementsHidden
      className={`items-center justify-center rounded-full ${sizeClassNames[size]} ${toneClassNames[tone]}`}
    >
      <AppText variant={textVariants[size]} tone={textTones[tone]}>
        {initialsOf(name)}
      </AppText>
    </View>
  );
}
