import { useQuery } from "@tanstack/react-query";
import { Redirect } from "expo-router";
import { useState } from "react";
import { FlatList, View } from "react-native";
import { isBackofficeSessionEnded } from "@/backoffice/api/backofficeHttpClient";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { CommandLogItem } from "@/backoffice/logs/CommandLogItem";
import {
  commandLogFiltersOf,
  matchesCommandLogFilters,
  matchesSecurityEventFilters,
  securityEventFiltersOf,
} from "@/backoffice/logs/logFilters";
import { todayUtcIsoDate } from "@/backoffice/logs/logLabels";
import {
  CommandLogEntry,
  commandLogQueryKey,
  getCommandLog,
  getSecurityEvents,
  SecurityEventEntry,
  securityEventsQueryKey,
} from "@/backoffice/logs/logsApi";
import { SecurityEventItem } from "@/backoffice/logs/SecurityEventItem";
import { addDays, formatLongDate } from "@/dates/dates";
import { ListFilterControls, ListFilterSummary } from "@/filters/ListFilterControls";
import { hasActiveFilters, ListFilterValues, noListFilterValues } from "@/filters/listFilters";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { EmptyState } from "@/ui/EmptyState";
import { IconButton } from "@/ui/IconButton";
import { ListDivider } from "@/ui/ListRow";
import { SkeletonList } from "@/ui/motion/SkeletonList";
import { Screen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SegmentedControl } from "@/ui/SegmentedControl";

type LogKind = "commands" | "securityEvents";

type LogRow =
  | { kind: "commands"; key: string; entry: CommandLogEntry }
  | { kind: "securityEvents"; key: string; entry: SecurityEventEntry };

const previousDay = -1;
const nextDay = 1;

function rowsOf(
  kind: LogKind,
  commandEntries: readonly CommandLogEntry[],
  securityEntries: readonly SecurityEventEntry[],
): LogRow[] {
  return kind === "commands"
    ? commandEntries.map((entry, index) => ({ kind, key: `${entry.occurredAt}-${index}`, entry }))
    : securityEntries.map((entry, index) => ({ kind, key: `${entry.occurredAt}-${index}`, entry }));
}

export function LogsScreen() {
  const today = todayUtcIsoDate();
  const [kind, setKind] = useState<LogKind>("commands");
  const [day, setDay] = useState(today);
  const [filterValues, setFilterValues] = useState<ListFilterValues>(noListFilterValues);
  const [expandedKey, setExpandedKey] = useState<string | null>(null);
  const commands = useQuery({
    queryKey: commandLogQueryKey(day),
    queryFn: () => getCommandLog(day),
    enabled: kind === "commands",
    retry: false,
  });
  const securityEvents = useQuery({
    queryKey: securityEventsQueryKey(day),
    queryFn: () => getSecurityEvents(day),
    enabled: kind === "securityEvents",
    retry: false,
  });
  const log = kind === "commands" ? commands : securityEvents;

  if (isBackofficeSessionEnded(log.error)) {
    return <Redirect href={backofficeRoutes.signIn} />;
  }

  const commandEntries = commands.data?.entries ?? [];
  const securityEntries = securityEvents.data?.events ?? [];
  const isTruncated = kind === "commands" && commands.data?.isTruncated === true;
  const filters =
    kind === "commands"
      ? commandLogFiltersOf(commandEntries)
      : securityEventFiltersOf(securityEntries);
  const rowsMatching = (values: ListFilterValues) =>
    kind === "commands"
      ? rowsOf(
          kind,
          commandEntries.filter((entry) => matchesCommandLogFilters(entry, values)),
          [],
        )
      : rowsOf(
          kind,
          [],
          securityEntries.filter((entry) => matchesSecurityEventFilters(entry, values)),
        );
  const allRows = rowsMatching(noListFilterValues);
  const visibleRows = rowsMatching(filterValues);
  const isFiltered = hasActiveFilters(filterValues);

  const changeKind = (nextKind: LogKind) => {
    setKind(nextKind);
    setFilterValues(noListFilterValues);
    setExpandedKey(null);
  };
  const changeDay = (days: number) => {
    setDay(addDays(day, days));
    setFilterValues(noListFilterValues);
    setExpandedKey(null);
  };
  const toggle = (key: string) => setExpandedKey(expandedKey === key ? null : key);

  return (
    <Screen>
      <View className="gap-3.5 pb-2">
        <ScreenHeader
          eyebrow={translate("backoffice.signIn.title")}
          title={translate("backoffice.tabs.logs")}
          caption={translate("backoffice.logs.day", { date: formatLongDate(day) })}
          accessory={
            <View className="flex-row">
              <IconButton
                icon="previous"
                accessibilityLabel={translate("backoffice.logs.previousDay")}
                onPress={() => changeDay(previousDay)}
              />
              <IconButton
                icon="next"
                accessibilityLabel={translate("backoffice.logs.nextDay")}
                disabled={day >= today}
                onPress={() => changeDay(nextDay)}
              />
            </View>
          }
        />
        <View className="gap-3.5 px-5">
          <SegmentedControl
            options={[
              { value: "commands", label: translate("backoffice.logs.commands") },
              { value: "securityEvents", label: translate("backoffice.logs.securityEvents") },
            ]}
            selectedValue={kind}
            onChange={changeKind}
          />
          {allRows.length === 0 ? null : (
            <ListFilterControls
              filters={filters}
              values={filterValues}
              onChange={setFilterValues}
              countMatching={(values) => rowsMatching(values).length}
            />
          )}
          {isFiltered ? (
            <ListFilterSummary shownCount={visibleRows.length} totalCount={allRows.length} />
          ) : null}
          {isTruncated ? (
            <Banner tone="warning" message={translate("backoffice.logs.truncated")} />
          ) : null}
          {log.isError ? (
            <Banner message={translate("common.unexpectedError")}>
              <Button
                variant="secondary"
                size="medium"
                label={translate("common.retry")}
                onPress={() => log.refetch()}
              />
            </Banner>
          ) : null}
        </View>
      </View>
      {log.isPending ? (
        <SkeletonList className="px-5" />
      ) : (
        <FlatList
          data={visibleRows}
          keyExtractor={(row) => row.key}
          ItemSeparatorComponent={() => (
            <View className="px-5">
              <ListDivider />
            </View>
          )}
          renderItem={({ item: row }) => (
            <View className="px-5">
              {row.kind === "commands" ? (
                <CommandLogItem
                  entry={row.entry}
                  isExpanded={expandedKey === row.key}
                  onToggle={() => toggle(row.key)}
                />
              ) : (
                <SecurityEventItem
                  entry={row.entry}
                  isExpanded={expandedKey === row.key}
                  onToggle={() => toggle(row.key)}
                />
              )}
            </View>
          )}
          ListEmptyComponent={
            log.isError ? null : (
              <EmptyState
                icon={isFiltered ? "search" : "orders"}
                message={
                  isFiltered
                    ? translate("filters.noResults")
                    : translate(
                        kind === "commands"
                          ? "backoffice.logs.noCommands"
                          : "backoffice.logs.noSecurityEvents",
                      )
                }
              />
            )
          }
          refreshing={log.isRefetching}
          onRefresh={() => log.refetch()}
          contentContainerClassName="w-full max-w-2xl self-center pb-28"
        />
      )}
    </Screen>
  );
}
