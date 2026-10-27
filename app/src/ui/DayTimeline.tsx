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
  for (let hour = firstHour; hour < lastHour; hour += stepHours) {
    hours.push(hour);
  }
  return [...hours, lastHour];
}

export function DayTimeline({
  startMinutes,
  endMinutes,
  periods,
  accessibilityLabel,
  legendLabels,
}: DayTimelineProps) {
  const visibleMinutes = Math.max(1, endMinutes - startMinutes);
  const percentageOf = (minutes: number): `${number}%` =>
    `${((Math.min(Math.max(minutes, startMinutes), endMinutes) - startMinutes) / visibleMinutes) * fullPercentage}%`;
  const widthOf = (period: DayTimelinePeriod): `${number}%` => {
    const visibleStart = Math.max(period.startMinutes, startMinutes);
    const visibleEnd = Math.min(period.endMinutes, endMinutes);
    return `${(Math.max(0, visibleEnd - visibleStart) / visibleMinutes) * fullPercentage}%`;
  };
  const drawnPeriods = toneOrder.flatMap((tone) =>
    periods.filter((period) => period.tone === tone),
  );
  const shownTones = toneOrder.filter((tone) => periods.some((period) => period.tone === tone));
  return (
    <View className="gap-2">
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
            style={{ left: percentageOf(period.startMinutes), width: widthOf(period) }}
          />
        ))}
      </View>
      <View className="h-4" importantForAccessibility="no-hide-descendants">
        {hourTicks(startMinutes, endMinutes).map((hour, tickIndex, hours) => {
          const isFirst = tickIndex === 0;
          const isLast = tickIndex === hours.length - 1;
          return (
            <AppText
              key={hour}
              variant="footnote"
              tone="subtle"
              className={`absolute ${isLast ? "-translate-x-full" : isFirst ? "" : "-translate-x-1/2"}`}
              style={{ left: percentageOf(hour * minutesPerHour) }}
            >
              {String(hour)}
            </AppText>
          );
        })}
      </View>
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
    </View>
  );
}
