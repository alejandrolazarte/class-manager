import { ReactNode } from "react";
import { View } from "react-native";
import { AppText } from "@/ui/AppText";

export type DayTimelineTone = "busy" | "free" | "chosen" | "conflict";

export interface DayTimelinePeriod {
  startMinutes: number;
  endMinutes: number;
  tone: DayTimelineTone;
}

interface DayTimelineProps {
  startMinutes: number;
  endMinutes: number;
  periods: DayTimelinePeriod[];
  accessibilityLabel: string;
  legendLabels: Record<DayTimelineTone, string>;
}

export interface WeekTimelineRow {
  key: string;
  label: string;
  periods: DayTimelinePeriod[];
  accessibilityLabel: string;
  caption?: ReactNode;
}

interface WeekTimelineProps {
  startMinutes: number;
  endMinutes: number;
  rows: WeekTimelineRow[];
  legendLabels: Record<DayTimelineTone, string>;
}

interface TimelineRange {
  startMinutes: number;
  endMinutes: number;
}

const toneOrder: readonly DayTimelineTone[] = ["busy", "free", "chosen", "conflict"];

const periodClassNames: Record<DayTimelineTone, string> = {
  busy: "top-0 bottom-0 rounded border border-surface bg-subtle-foreground",
  free: "top-0 bottom-0 rounded border-[1.5px] border-primary bg-primary-soft",
  chosen: "-top-1 -bottom-1 rounded-md border-2 border-surface bg-primary",
  conflict: "-top-1 -bottom-1 rounded-md border-2 border-surface bg-danger",
};

const swatchClassNames: Record<DayTimelineTone, string> = {
  busy: "bg-subtle-foreground",
  free: "border-[1.5px] border-primary bg-primary-soft",
  chosen: "bg-primary",
  conflict: "bg-danger",
};

const minutesPerHour = 60;
const maximumTickCount = 5;
const fullPercentage = 100;

function hourTicks(startMinutes: number, endMinutes: number): number[] {
  const firstHour = Math.ceil(startMinutes / minutesPerHour);
  const lastHour = Math.floor(endMinutes / minutesPerHour);
  const stepHours = Math.max(1, Math.ceil((lastHour - firstHour) / (maximumTickCount - 1)));
  const hours: number[] = [];
  for (let hour = firstHour; lastHour - hour > stepHours / 2; hour += stepHours) {
    hours.push(hour);
  }
  return [...hours, lastHour];
}

function percentageWithin(range: TimelineRange, minutes: number): `${number}%` {
  const visibleMinutes = Math.max(1, range.endMinutes - range.startMinutes);
  const clampedMinutes = Math.min(Math.max(minutes, range.startMinutes), range.endMinutes);
  return `${((clampedMinutes - range.startMinutes) / visibleMinutes) * fullPercentage}%`;
}

function widthWithin(range: TimelineRange, period: DayTimelinePeriod): `${number}%` {
  const visibleMinutes = Math.max(1, range.endMinutes - range.startMinutes);
  const visibleStart = Math.max(period.startMinutes, range.startMinutes);
  const visibleEnd = Math.min(period.endMinutes, range.endMinutes);
  return `${(Math.max(0, visibleEnd - visibleStart) / visibleMinutes) * fullPercentage}%`;
}

function TimelineBar({
  range,
  periods,
  accessibilityLabel,
}: {
  range: TimelineRange;
  periods: DayTimelinePeriod[];
  accessibilityLabel: string;
}) {
  const drawnPeriods = toneOrder.flatMap((tone) =>
    periods.filter((period) => period.tone === tone),
  );
  return (
    <View
      accessible
      accessibilityRole="image"
      accessibilityLabel={accessibilityLabel}
      className="h-6 rounded-md bg-muted"
    >
      {drawnPeriods.map((period) => (
        <View
          key={`${period.tone}-${period.startMinutes}`}
          testID={`day-timeline-period-${period.tone}`}
          className={`absolute ${periodClassNames[period.tone]}`}
          style={{
            left: percentageWithin(range, period.startMinutes),
            width: widthWithin(range, period),
          }}
        />
      ))}
    </View>
  );
}

function HourTicks({ range, className }: { range: TimelineRange; className?: string }) {
  return (
    <View className={`h-4 ${className ?? ""}`} importantForAccessibility="no-hide-descendants">
      {hourTicks(range.startMinutes, range.endMinutes).map((hour, tickIndex, hours) => {
        const isFirst = tickIndex === 0;
        const isLast = tickIndex === hours.length - 1;
        return (
          <AppText
            key={hour}
            variant="footnote"
            tone="subtle"
            className={`absolute ${isLast ? "-translate-x-full" : isFirst ? "" : "-translate-x-1/2"}`}
            style={{ left: percentageWithin(range, hour * minutesPerHour) }}
          >
            {String(hour)}
          </AppText>
        );
      })}
    </View>
  );
}

function TimelineLegend({
  periods,
  legendLabels,
}: {
  periods: DayTimelinePeriod[];
  legendLabels: Record<DayTimelineTone, string>;
}) {
  const shownTones = toneOrder.filter((tone) => periods.some((period) => period.tone === tone));
  return (
    <View className="flex-row flex-wrap gap-3">
      {shownTones.map((tone) => (
        <View key={tone} className="flex-row items-center gap-1.5">
          <View className={`h-3 w-3 rounded-sm ${swatchClassNames[tone]}`} />
          <AppText variant="footnote" tone="muted">
            {legendLabels[tone]}
          </AppText>
        </View>
      ))}
    </View>
  );
}

export function DayTimeline({
  startMinutes,
  endMinutes,
  periods,
  accessibilityLabel,
  legendLabels,
}: DayTimelineProps) {
  const range = { startMinutes, endMinutes };
  return (
    <View className="gap-2">
      <TimelineBar range={range} periods={periods} accessibilityLabel={accessibilityLabel} />
      <HourTicks range={range} />
      <TimelineLegend periods={periods} legendLabels={legendLabels} />
    </View>
  );
}

export function WeekTimeline({ startMinutes, endMinutes, rows, legendLabels }: WeekTimelineProps) {
  const range = { startMinutes, endMinutes };
  return (
    <View className="gap-2">
      {rows.map((row) => (
        <View key={row.key} className="gap-1">
          <View className="flex-row items-center gap-3">
            <AppText variant="label" className="w-10">
              {row.label}
            </AppText>
            <View className="flex-1">
              <TimelineBar
                range={range}
                periods={row.periods}
                accessibilityLabel={row.accessibilityLabel}
              />
            </View>
          </View>
          {row.caption ? <View className="pl-[52px]">{row.caption}</View> : null}
        </View>
      ))}
      <HourTicks range={range} className="ml-[52px]" />
      <TimelineLegend periods={rows.flatMap((row) => row.periods)} legendLabels={legendLabels} />
    </View>
  );
}
