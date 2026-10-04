import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";

export function RequiredFieldsLegend() {
  return (
    <AppText variant="caption" tone="subtle">
      {translate("common.requiredLegend")}
    </AppText>
  );
}
