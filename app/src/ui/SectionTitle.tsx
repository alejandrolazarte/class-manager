import { AppText } from "@/ui/AppText";

interface SectionTitleProps {
  title: string;
  isOverline?: boolean;
}

export function SectionTitle({ title, isOverline = false }: SectionTitleProps) {
  return (
    <AppText
      variant={isOverline ? "overline" : "heading"}
      tone={isOverline ? "subtle" : "default"}
      accessibilityRole="header"
      className={isOverline ? "" : "mt-1"}
    >
      {title}
    </AppText>
  );
}
