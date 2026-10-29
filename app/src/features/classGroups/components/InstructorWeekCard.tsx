import { View } from "react-native";
import { weekdayLongLabel, weekdayShortLabel, Weekday } from "@/dates/weekdays";
import {
  InstructorWeek,
  InstructorWeekday,
  planInstructorWeek,
} from "@/features/classGroups/instructorWeek";
import { formatMinutesOfDay } from "@/features/classGroups/startTimeFormatting";
import { useInstructorWeek } from "@/features/classGroups/useInstructorWeek";
import { freeGapLabel } from "@/features/privateLessons/freeGapLabel";
import { BusyPeriod, isLessonInsideGap } from "@/features/privateLessons/instructorDay";
import { firstNameOf } from "@/features/studentApp/studentAppSchedule";
import { joinNames } from "@/features/studentApp/studentAppNewsPresentation";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Chip } from "@/ui/Chip";
import { DayTimelinePeriod, WeekTimeline, WeekTimelineRow } from "@/ui/DayTimeline";
import { Icon } from "@/ui/Icon";
import { OutlinedBlock } from "@/ui/OutlinedBlock";
import { StatusPill } from "@/ui/StatusPill";
import { SkeletonList } from "@/ui/motion/SkeletonList";

interface InstructorWeekCardProps {
  instructorId: string;
  instructorFullName: string;
  weekdays: Weekday[];
  durationMinutes: number | null;
  startTime: string;
  ignoredClassGroupId?: string;
  onPickStartTime: (startTime: string) => void;
}

const loadingRowCount = 1;
const singleDayCount = 1;

function busyName(busyPeriod: BusyPeriod): string {
  return busyPeriod.slot.classGroupName ?? translate("privateLessons.agenda.privateLesson");
}

function timelinePeriods(day: InstructorWeekday): DayTimelinePeriod[] {
  const { lesson } = day;
  return [
    ...day.busyPeriods.map((busyPeriod): DayTimelinePeriod => ({
      startMinutes: busyPeriod.startMinutes,
      endMinutes: busyPeriod.endMinutes,
      tone: "busy",
    })),
    ...(lesson === null
      ? []
      : [
          {
            startMinutes: lesson.startMinutes,
            endMinutes: lesson.endMinutes,
            tone: lesson.conflict === null ? "chosen" : "conflict",
          } satisfies DayTimelinePeriod,
        ]),
  ];
}

function dayAccessibilityLabel(day: InstructorWeekday): string {
  const weekday = weekdayLongLabel(day.weekday);
  if (day.busyPeriods.length === 0) {
    return translate("classGroups.instructorWeek.timelineFreeDay", { day: weekday });
  }
  return translate("classGroups.instructorWeek.timelineLabel", {
    day: weekday,
    classes: day.busyPeriods
      .map((busyPeriod) =>
        translate("privateLessons.agenda.timelineClass", {
          start: formatMinutesOfDay(busyPeriod.startMinutes),
          end: formatMinutesOfDay(busyPeriod.endMinutes),
        }),
      )
      .join(", "),
  });
}

function DayStatus({ day }: { day: InstructorWeekday }) {
  const { lesson } = day;
  if (lesson === null) {
    return null;
  }
  if (lesson.conflict !== null) {
    return (
      <View accessibilityRole="text" className="flex-row items-center gap-1.5">
        <Icon name="warning" size="small" tone="danger" />
        <AppText variant="caption" tone="danger" className="flex-1">
          {translate("classGroups.instructorWeek.conflict", {
            name: busyName(lesson.conflict),
            busyStart: formatMinutesOfDay(lesson.conflict.startMinutes),
            busyEnd: formatMinutesOfDay(lesson.conflict.endMinutes),
          })}
        </AppText>
      </View>
    );
  }
  return (
    <View accessibilityRole="text" className="flex-row items-center gap-1.5">
      <Icon name="fieldValid" size="small" tone="primary" />
      <AppText variant="caption" tone="primary" className="flex-1">
        {translate("classGroups.instructorWeek.fits", {
          start: formatMinutesOfDay(lesson.startMinutes),
          end: formatMinutesOfDay(lesson.endMinutes),
        })}
      </AppText>
    </View>
  );
}

function WeekStatus({
  instructorWeek,
  instructorName,
}: {
  instructorWeek: InstructorWeek;
  instructorName: string;
}) {
  const dayCount = instructorWeek.days.length;
  if (!instructorWeek.isClassPlanned) {
    return (
      <AppText variant="caption" tone="subtle">
        {translate("classGroups.instructorWeek.pickStartTime")}
      </AppText>
    );
  }
  if (instructorWeek.clashingDayCount > 0) {
    return (
      <Banner
        tone="warning"
        message={
          dayCount === singleDayCount
            ? translate("classGroups.instructorWeek.conflictOnTheDay", { name: instructorName })
            : translate("classGroups.instructorWeek.conflictSummary", {
                count: instructorWeek.clashingDayCount,
                total: dayCount,
              })
        }
      />
    );
  }
  return (
    <View accessibilityRole="text" className="flex-row items-center gap-1.5">
      <Icon name="fieldValid" size="small" tone="primary" />
      <AppText variant="link" tone="primary" className="flex-1">
        {dayCount === singleDayCount
          ? translate("classGroups.instructorWeek.fitsTheDay")
          : translate("classGroups.instructorWeek.fitsEveryDay", { count: dayCount })}
      </AppText>
    </View>
  );
}

function FreeTimes({
  instructorWeek,
  durationMinutes,
  dayNames,
  onPickStartTime,
}: {
  instructorWeek: InstructorWeek;
  durationMinutes: number;
  dayNames: string;
  onPickStartTime: (startTime: string) => void;
}) {
  const isSingleDay = instructorWeek.days.length === singleDayCount;
  const lessonOfTheFirstDay = instructorWeek.days[0]?.lesson ?? null;
  return (
    <View className="gap-2">
      <AppText variant="label" tone="muted">
        {isSingleDay
          ? translate("privateLessons.agenda.freeFor", { minutes: durationMinutes })
          : translate("classGroups.instructorWeek.freeOnEveryDay", {
              days: dayNames,
              minutes: durationMinutes,
            })}
      </AppText>
      {instructorWeek.freeGapsOnEveryDay.length === 0 ? (
        <AppText variant="caption" tone="subtle">
          {isSingleDay
            ? translate("privateLessons.agenda.noGaps", { minutes: durationMinutes })
            : translate("classGroups.instructorWeek.noGapsOnEveryDay", {
                minutes: durationMinutes,
              })}
        </AppText>
      ) : (
        <>
          <View className="flex-row flex-wrap gap-2">
            {instructorWeek.freeGapsOnEveryDay.map((gap) => (
              <Chip
                key={gap.startMinutes}
                label={freeGapLabel(gap)}
                isSelected={isLessonInsideGap(lessonOfTheFirstDay, gap)}
                onPress={() => onPickStartTime(formatMinutesOfDay(gap.suggestedStartMinutes))}
              />
            ))}
          </View>
          {instructorWeek.isClassPlanned ? null : (
            <AppText variant="caption" tone="subtle">
              {translate("privateLessons.agenda.pickHint")}
            </AppText>
          )}
        </>
      )}
    </View>
  );
}

export function InstructorWeekCard({
  instructorId,
  instructorFullName,
  weekdays,
  durationMinutes,
  startTime,
  ignoredClassGroupId,
  onPickStartTime,
}: InstructorWeekCardProps) {
  const weekQuery = useInstructorWeek(instructorId, ignoredClassGroupId);
  if (instructorId.length === 0 || weekQuery.isError) {
    return null;
  }
  const instructorName = firstNameOf(instructorFullName);
  if (weekdays.length === 0) {
    return (
      <OutlinedBlock className="bg-surface">
        <AppText variant="caption" tone="muted">
          {translate("classGroups.instructorWeek.pickWeekdays", { name: instructorName })}
        </AppText>
      </OutlinedBlock>
    );
  }
  if (weekQuery.isPending) {
    return <SkeletonList rowCount={loadingRowCount} />;
  }
  const instructorWeek = planInstructorWeek(weekQuery.data, weekdays, durationMinutes, startTime);
  const dayCount = instructorWeek.days.length;
  if (!instructorWeek.hasClasses) {
    return (
      <OutlinedBlock className="bg-surface">
        <AppText variant="caption" tone="muted">
          {dayCount === singleDayCount
            ? translate("privateLessons.agenda.freeDay", { name: instructorName })
            : translate("classGroups.instructorWeek.freeDays", { name: instructorName })}
        </AppText>
      </OutlinedBlock>
    );
  }
  const dayNames = joinNames(instructorWeek.days.map((day) => weekdayShortLabel(day.weekday)));
  const rows = instructorWeek.days.map((day): WeekTimelineRow => ({
    key: day.weekday,
    label: weekdayShortLabel(day.weekday),
    periods: timelinePeriods(day),
    accessibilityLabel: dayAccessibilityLabel(day),
    caption: <DayStatus day={day} />,
  }));
  return (
    <OutlinedBlock className="gap-3 bg-surface">
      <View className="flex-row items-start justify-between gap-2">
        <AppText variant="bodyStrong" className="flex-1">
          {translate("classGroups.instructorWeek.title", { name: instructorName, days: dayNames })}
        </AppText>
        <StatusPill
          tone="neutral"
          isSmall
          label={translateCount("classGroups.instructorWeek.dayCount", dayCount)}
        />
      </View>
      <WeekTimeline
        startMinutes={instructorWeek.visibleStartMinutes}
        endMinutes={instructorWeek.visibleEndMinutes}
        rows={rows}
        legendLabels={{
          busy: translate("privateLessons.agenda.busy"),
          free: translate("privateLessons.agenda.free"),
          chosen: translate("classGroups.instructorWeek.thisClass"),
          conflict: translate("classGroups.instructorWeek.thisClass"),
        }}
      />
      <WeekStatus instructorWeek={instructorWeek} instructorName={instructorName} />
      {durationMinutes === null ? null : (
        <FreeTimes
          instructorWeek={instructorWeek}
          durationMinutes={durationMinutes}
          dayNames={dayNames}
          onPickStartTime={onPickStartTime}
        />
      )}
    </OutlinedBlock>
  );
}
