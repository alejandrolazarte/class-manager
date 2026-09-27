import { Text, TextProps } from "react-native";

export const textVariantClassNames = {
  hero: "font-heavy text-[30px] leading-[33px] tracking-[-0.6px]",
  display: "font-heavy text-[26px] leading-[30px] tracking-[-0.5px]",
  headline: "font-heavy text-[22px] leading-[26px] tracking-[-0.4px]",
  amount: "font-heavy text-[32px] leading-[36px] tracking-[-0.6px]",
  title: "font-heavy text-xl",
  heading: "font-strong text-[17px] leading-[22px]",
  lead: "font-text text-base leading-[23px]",
  body: "font-text text-[15px] leading-[21px]",
  bodyStrong: "font-strong text-[15px] leading-[20px]",
  button: "font-strong text-base",
  link: "font-strong text-sm",
  eyebrow: "font-strong text-[13px] leading-4",
  label: "font-label text-[13px]",
  caption: "font-text text-[13px] leading-[17px]",
  footnote: "font-text text-xs",
  badge: "font-strong text-xs",
  overline: "font-strong text-xs uppercase tracking-[0.7px]",
} as const;

export const textToneClassNames = {
  default: "text-foreground",
  muted: "text-muted-foreground",
  subtle: "text-subtle-foreground",
  disabled: "text-disabled-foreground",
  primary: "text-primary",
  primarySoft: "text-primary-soft-foreground",
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
