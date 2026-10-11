import { View } from "react-native";
import { AppText, TextTone } from "@/ui/AppText";
import { LivePulse } from "@/ui/motion/LivePulse";
import { PressableScale } from "@/ui/motion/PressableScale";

export type TimelineTone = "live" | "done" | "pending" | "upcoming" | "cancelled";

interface TimelineRowProps {
  startTime: string;
  endTime: string;
  title: string;
  meta: string;
  status: string;
  detail?: string;
  tone: TimelineTone;
  isFirst: boolean;
  isLast: boolean;
  onPress: () => void;
  accessibilityLabel: string;
}

const dotClassNames: Record<TimelineTone, string> = {
  live: "border-primary bg-primary",
  done: "border-success bg-success",
  pending: "border-warning bg-warning",
  upcoming: "border-subtle-foreground bg-background",
  cancelled: "border-disabled-foreground bg-background",
};

const statusTones: Record<TimelineTone, TextTone> = {
  live: "primary",
  done: "success",
  pending: "warning",
  upcoming: "muted",
  cancelled: "subtle",
};

const dimmedTones = new Set<TimelineTone>(["done", "cancelled"]);

function lineClassName(isFirst: boolean, isLast: boolean): string | null {
  if (isFirst && isLast) {
    return null;
  }
  if (isFirst) {
    return "top-[22px] bottom-0";
  }
  return isLast ? "top-0 h-[22px]" : "top-0 bottom-0";
}

export function TimelineRow({
  startTime,
  endTime,
  title,
  meta,
  status,
  detail,
  tone,
  isFirst,
  isLast,
  onPress,
  accessibilityLabel,
}: TimelineRowProps) {
  const line = lineClassName(isFirst, isLast);
  const dot = <View className={`h-2 w-2 rounded-full border-[1.5px] ${dotClassNames[tone]}`} />;
  return (
    <PressableScale
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel}
      onPress={onPress}
      className={`-mx-2 h-16 flex-row gap-2 rounded-xl px-2 active:bg-muted ${dimmedTones.has(tone) ? "opacity-60" : ""}`}
    >
      <View className="w-12 pt-3">
        <AppText variant="bodyStrong">{startTime}</AppText>
        <AppText variant="footnote" tone="subtle">
          {endTime}
        </AppText>
      </View>
      <View className="relative w-[18px] self-stretch">
        {line === null ? null : (
          <View className={`absolute left-2 w-[1.5px] bg-border-subtle ${line}`} />
        )}
        <View className="absolute left-[5px] top-[18px]">
          {tone === "live" ? (
            <LivePulse ringClassName="rounded-full bg-primary">{dot}</LivePulse>
          ) : (
            dot
          )}
        </View>
      </View>
      <View className="min-w-0 flex-1 flex-row gap-2 border-b border-border-subtle pb-3 pt-2.5">
        <View className="min-w-0 flex-1">
          <AppText
            variant="bodyStrong"
            tone={tone === "cancelled" ? "muted" : "default"}
            numberOfLines={1}
            className={tone === "cancelled" ? "line-through" : ""}
          >
            {title}
          </AppText>
          <AppText variant="caption" tone="subtle" numberOfLines={1}>
            {meta}
          </AppText>
        </View>
        <View className="items-end pt-0.5">
          <AppText variant="badge" tone={statusTones[tone]} numberOfLines={1}>
            {status}
          </AppText>
          {detail ? (
            <AppText variant="footnote" tone="subtle" numberOfLines={1}>
              {detail}
            </AppText>
          ) : null}
        </View>
      </View>
    </PressableScale>
  );
}
