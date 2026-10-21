import { View } from "react-native";
import { LogDetails } from "@/backoffice/logs/LogDetails";
import {
  commandLabel,
  formatUtcTime,
  outcomeLabel,
  sourceLabel,
} from "@/backoffice/logs/logLabels";
import { CommandLogEntry } from "@/backoffice/logs/logsApi";
import { translate } from "@/i18n/translate";
import { ListRow } from "@/ui/ListRow";
import { Collapsible } from "@/ui/motion/Collapsible";

const succeededOutcome = "Succeeded";
const backofficeSource = "Backoffice";
const detailSeparator = " · ";

function summaryOf(entry: CommandLogEntry): string {
  return [
    formatUtcTime(entry.occurredAt),
    entry.brandName,
    entry.branchName,
    entry.userName,
    entry.source === backofficeSource ? sourceLabel(entry.source) : null,
    entry.outcome === succeededOutcome ? null : outcomeLabel(entry.outcome),
  ]
    .filter(Boolean)
    .join(detailSeparator);
}

function errorOf(entry: CommandLogEntry): string | null {
  return [entry.errorKind, entry.errorCode].filter(Boolean).join(detailSeparator) || null;
}

interface CommandLogItemProps {
  entry: CommandLogEntry;
  isExpanded: boolean;
  onToggle: () => void;
}

export function CommandLogItem({ entry, isExpanded, onToggle }: CommandLogItemProps) {
  return (
    <View>
      <ListRow
        label={commandLabel(entry.command)}
        detail={summaryOf(entry)}
        isDestructive={entry.outcome !== succeededOutcome}
        isExpanded={isExpanded}
        onPress={onToggle}
      />
      <Collapsible isExpanded={isExpanded}>
        <LogDetails
          details={[
            {
              label: translate("backoffice.logs.detail.outcome"),
              value: outcomeLabel(entry.outcome),
            },
            { label: translate("backoffice.logs.detail.error"), value: errorOf(entry) },
            { label: translate("backoffice.logs.detail.source"), value: sourceLabel(entry.source) },
            { label: translate("backoffice.logs.detail.brand"), value: entry.brandName },
            { label: translate("backoffice.logs.detail.branch"), value: entry.branchName },
            { label: translate("backoffice.logs.detail.user"), value: entry.userName },
            {
              label: translate("backoffice.logs.detail.duration"),
              value: translate("backoffice.logs.milliseconds", {
                count: entry.durationMilliseconds,
              }),
            },
            { label: translate("backoffice.logs.detail.rows"), value: entry.rowCount },
            { label: translate("backoffice.logs.detail.command"), value: entry.command },
            { label: translate("backoffice.logs.detail.trace"), value: entry.traceId },
            {
              label: translate("backoffice.logs.detail.payload"),
              value: entry.payloadTruncated ? translate("backoffice.logs.payloadTruncated") : null,
            },
          ]}
          payload={entry.payload}
        />
      </Collapsible>
    </View>
  );
}
