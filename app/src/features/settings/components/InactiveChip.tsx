import { translate } from "@/i18n/translate";
import { StatusPill } from "@/ui/StatusPill";

export function InactiveChip() {
  return <StatusPill label={translate("common.inactive")} tone="neutral" isSmall />;
}
