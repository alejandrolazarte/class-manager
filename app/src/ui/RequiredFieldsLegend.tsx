import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";

export function withRequiredMark(label: string, isRequired = true): string {
  return isRequired ? translate("common.requiredLabel", { label }) : label;
}

export function RequiredFieldsLegend() {
  return (
    <AppText variant="caption" tone="subtle">
      {translate("common.requiredLegend")}
    </AppText>
  );
}
