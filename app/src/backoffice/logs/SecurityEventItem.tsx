import { View } from "react-native";
import { LogDetails } from "@/backoffice/logs/LogDetails";
import { eventLabel, formatUtcTime } from "@/backoffice/logs/logLabels";
import { SecurityEventEntry } from "@/backoffice/logs/logsApi";
import { translate } from "@/i18n/translate";
import { ListRow } from "@/ui/ListRow";
import { Collapsible } from "@/ui/motion/Collapsible";

const detailSeparator = " · ";

function requestOf(entry: SecurityEventEntry): string | null {
  return [entry.method, entry.route, entry.statusCode].filter(Boolean).join(" ") || null;
}

function summaryOf(entry: SecurityEventEntry): string {
  return [
    formatUtcTime(entry.occurredAt),
    requestOf(entry),
    entry.ipAddress,
    entry.brandName,
    entry.count === null ? null : translate("backoffice.logs.times", { count: entry.count }),
  ]
    .filter(Boolean)
    .join(detailSeparator);
}

interface SecurityEventItemProps {
  entry: SecurityEventEntry;
  isExpanded: boolean;
  onToggle: () => void;
}

export function SecurityEventItem({ entry, isExpanded, onToggle }: SecurityEventItemProps) {
  return (
    <View>
      <ListRow
        label={eventLabel(entry.event)}
        detail={summaryOf(entry)}
        isExpanded={isExpanded}
        onPress={onToggle}
      />
      <Collapsible isExpanded={isExpanded}>
        <LogDetails
          details={[
            { label: translate("backoffice.logs.detail.request"), value: requestOf(entry) },
            { label: translate("backoffice.logs.detail.brand"), value: entry.brandName },
            { label: translate("backoffice.logs.detail.branch"), value: entry.branchName },
            { label: translate("backoffice.logs.detail.user"), value: entry.userName },
            { label: translate("backoffice.logs.detail.ipAddress"), value: entry.ipAddress },
            { label: translate("backoffice.logs.detail.userAgent"), value: entry.userAgent },
            { label: translate("backoffice.logs.detail.trace"), value: entry.traceId },
          ]}
        />
      </Collapsible>
    </View>
  );
}
