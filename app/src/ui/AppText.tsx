import { Text, TextProps } from "react-native";

export const textVariantClassNames = {
  display: "text-2xl font-bold",
  headline: "text-xl font-bold",
  title: "text-xl font-semibold",
  heading: "text-lg font-semibold",
  lead: "text-lg",
  body: "text-base",
  bodyStrong: "text-base font-semibold",
  link: "text-base font-medium",
  label: "text-sm font-medium",
  caption: "text-sm",
  footnote: "text-xs",
  badge: "text-xs font-medium",
} as const;

export const textToneClassNames = {
  default: "text-foreground",
  muted: "text-muted-foreground",
  subtle: "text-subtle-foreground",
  disabled: "text-disabled-foreground",
  primary: "text-primary",
  onPrimary: "text-primary-foreground",
  danger: "text-danger",
  onDanger: "text-danger-foreground",
  dangerSoft: "text-danger-soft-foreground",
  warning: "text-warning",
  warningSoft: "text-warning-soft-foreground",
  success: "text-success",
  successSoft: "text-success-soft-foreground",
  inverse: "text-inverse-foreground",
} as const;

export type TextVariant = keyof typeof textVariantClassNames;

export type TextTone = keyof typeof textToneClassNames;

interface AppTextProps extends TextProps {
  variant?: TextVariant;
  tone?: TextTone;
  className?: string;
}

export function AppText({ variant, tone = "default", className, ...textProps }: AppTextProps) {
  const classNames = [
    variant ? textVariantClassNames[variant] : undefined,
    textToneClassNames[tone],
    className,
  ]
    .filter(Boolean)
    .join(" ");
  return <Text className={classNames} {...textProps} />;
}
