import { Image, StyleSheet, View } from "react-native";
import { AppText, TextTone, TextVariant } from "@/ui/AppText";

export type AvatarTone = "primarySoft" | "primary" | "onPrimary" | "muted" | "success" | "danger";

type AvatarSize = "tiny" | "small" | "medium" | "large" | "extraLarge";

interface AvatarProps {
  name: string;
  tone?: AvatarTone;
  size?: AvatarSize;
  photoUrl?: string | null;
}

const maximumInitials = 2;
const nameSeparatorPattern = /\s+/;

const toneClassNames: Record<AvatarTone, string> = {
  primarySoft: "bg-primary-soft",
  primary: "bg-primary",
  onPrimary: "bg-primary-foreground",
  muted: "bg-muted",
  success: "bg-success-soft",
  danger: "bg-danger-soft",
};

const textTones: Record<AvatarTone, TextTone> = {
  primarySoft: "primarySoft",
  primary: "onPrimary",
  onPrimary: "primary",
  muted: "muted",
  success: "successSoft",
  danger: "dangerSoft",
};

const sizeClassNames: Record<AvatarSize, string> = {
  tiny: "h-8 w-8",
  small: "h-10 w-10",
  medium: "h-11 w-11",
  large: "h-[60px] w-[60px]",
  extraLarge: "h-[88px] w-[88px]",
};

const textVariants: Record<AvatarSize, TextVariant> = {
  tiny: "link",
  small: "link",
  medium: "bodyStrong",
  large: "headline",
  extraLarge: "display",
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

export function Avatar({
  name,
  tone = "primarySoft",
  size = "medium",
  photoUrl = null,
}: AvatarProps) {
  return (
    <View
      importantForAccessibility="no-hide-descendants"
      accessibilityElementsHidden
      className={`items-center justify-center overflow-hidden rounded-full ${sizeClassNames[size]} ${toneClassNames[tone]}`}
    >
      {photoUrl === null ? (
        <AppText variant={textVariants[size]} tone={textTones[tone]} className="font-heavy">
          {initialsOf(name)}
        </AppText>
      ) : (
        <Image
          testID="avatar-photo"
          source={{ uri: photoUrl }}
          resizeMode="cover"
          style={StyleSheet.absoluteFill}
        />
      )}
    </View>
  );
}
