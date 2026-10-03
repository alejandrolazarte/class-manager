import { Text, TextProps } from "react-native";

export const textVariantClassNames = {
  hero: "font-heavy text-hero tracking-[-0.6px]",
  amount: "font-heavy text-hero tracking-[-0.6px]",
  display: "font-heavy text-display tracking-[-0.5px]",
  headline: "font-heavy text-title tracking-[-0.4px]",
  heading: "font-strong text-large",
  lead: "font-text text-large",
  button: "font-strong text-large",
  input: "font-label text-large",
  body: "font-text text-body",
  bodyStrong: "font-strong text-body",
  link: "font-strong text-caption",
  eyebrow: "font-strong text-caption",
  label: "font-label text-caption",
  caption: "font-text text-caption",
  footnote: "font-text text-small",
  badge: "font-strong text-small",
  overline: "font-strong text-small uppercase tracking-[0.7px]",
  counter: "font-strong text-micro",
} as const;

export const textToneClassNames = {
  default: "text-foreground",
  muted: "text-muted-foreground",
  subtle: "text-subtle-foreground",
  disabled: "text-disabled-foreground",
  primary: "text-primary",
  primarySoft: "text-primary-soft-foreground",
  accent: "text-accent",
  accentSoft: "text-accent-soft-foreground",
  onPrimary: "text-primary-foreground",
  danger: "text-danger",
  onDanger: "text-danger-foreground",
  dangerSoft: "text-danger-soft-foreground",
  warning: "text-warning",
  warningSoft: "text-warning-soft-foreground",
  success: "text-success",
  onSuccess: "text-success-foreground",
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

export function AppText({
  variant = "body",
  tone = "default",
  className,
  ...textProps
}: AppTextProps) {
  const classNames = [textVariantClassNames[variant], textToneClassNames[tone], className]
    .filter(Boolean)
    .join(" ");
  return <Text className={classNames} {...textProps} />;
}
