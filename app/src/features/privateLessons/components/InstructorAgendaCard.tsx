import { useState } from "react";
import { View } from "react-native";
import { formatMinutesOfDay } from "@/features/classGroups/startTimeFormatting";
import {
  BusyPeriod,
  FreeGap,
  InstructorDay,
  planInstructorDay,
} from "@/features/privateLessons/instructorDay";
import { InstructorAgendaSlot } from "@/features/privateLessons/types";
import { useInstructorAgenda } from "@/features/privateLessons/useInstructorAgenda";
import { formatLongDate } from "@/dates/dates";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Chip } from "@/ui/Chip";
import { DayTimeline, DayTimelinePeriod } from "@/ui/DayTimeline";
import { Icon } from "@/ui/Icon";
import { OutlinedBlock } from "@/ui/OutlinedBlock";
import { StatusPill } from "@/ui/StatusPill";
import { TextLink } from "@/ui/TextLink";
import { Collapsible } from "@/ui/motion/Collapsible";
import { SkeletonList } from "@/ui/motion/SkeletonList";

interface InstructorAgendaCardProps {
  instructorId: string;
  instructorFullName: string;
  date: string | null;
  durationMinutes: number | null;
  startTime: string;
  ignoredPrivateLessonId?: string;
  onPickStartTime: (startTime: string) => void;
}

const nameSeparator = " ";
const loadingRowCount = 1;

function firstNameOf(fullName: string): string {
  return fullName.split(nameSeparator)[0] ?? fullName;
}

function slotName(slot: InstructorAgendaSlot, privateLessonName: string): string {
  return slot.classGroupName ?? privateLessonName;
}

function gapLabel(gap: FreeGap): string {
  if (gap.isOpenStart) {
    return translate("privateLessons.agenda.gapUntil", {
      time: formatMinutesOfDay(gap.endMinutes),
    });
  }
  if (gap.isOpenEnd) {
    return translate("privateLessons.agenda.gapFrom", {
      time: formatMinutesOfDay(gap.startMinutes),
    });
  }
  return translate("privateLessons.agenda.gapBetween", {
    start: formatMinutesOfDay(gap.startMinutes),
    end: formatMinutesOfDay(gap.endMinutes),
  });
}

function isLessonInside(instructorDay: InstructorDay, gap: FreeGap): boolean {
  const { lesson } = instructorDay;
  return (
    lesson !== null &&
    lesson.conflict === null &&
    lesson.startMinutes >= gap.startMinutes &&
    lesson.endMinutes <= gap.endMinutes
  );
}

function timelinePeriods(instructorDay: InstructorDay): DayTimelinePeriod[] {
  const { lesson } = instructorDay;
  return [
    ...instructorDay.freeGaps.map((gap): DayTimelinePeriod => ({ ...gap, tone: "free" })),
    ...instructorDay.busyPeriods.map((busyPeriod): DayTimelinePeriod => ({
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

function periodRange(busyPeriod: BusyPeriod): string {
  return translate("privateLessons.agenda.gapBetween", {
    start: formatMinutesOfDay(busyPeriod.startMinutes),
    end: formatMinutesOfDay(busyPeriod.endMinutes),
  });
}

function LessonStatus({ instructorDay }: { instructorDay: InstructorDay }) {
  const { lesson } = instructorDay;
  if (lesson === null) {
    return instructorDay.freeGaps.length > 0 ? (
      <AppText variant="caption" tone="subtle">
        {translate("privateLessons.agenda.pickHint")}
      </AppText>
    ) : null;
  }
  const start = formatMinutesOfDay(lesson.startMinutes);
  const end = formatMinutesOfDay(lesson.endMinutes);
  if (lesson.conflict !== null) {
    return (
      <Banner
        message={translate("privateLessons.agenda.conflict", {
          start,
          end,
          name: slotName(lesson.conflict.slot, translate("privateLessons.agenda.privateLesson")),
          busyStart: formatMinutesOfDay(lesson.conflict.startMinutes),
          busyEnd: formatMinutesOfDay(lesson.conflict.endMinutes),
        })}
      />
    );
  }
  return (
    <View accessibilityRole="text" className="flex-row items-center gap-1.5">
      <Icon name="fieldValid" size="small" tone="primary" />
      <AppText variant="link" tone="primary" className="flex-1">
        {translate("privateLessons.agenda.fits", { start, end })}
      </AppText>
    </View>
  );
}

export function InstructorAgendaCard({
  instructorId,
  instructorFullName,
  date,
  durationMinutes,
  startTime,
  ignoredPrivateLessonId,
  onPickStartTime,
}: InstructorAgendaCardProps) {
  const [areClassesShown, setAreClassesShown] = useState(false);
  const agendaQuery = useInstructorAgenda(instructorId, date, ignoredPrivateLessonId);
  if (instructorId.length === 0 || date === null || agendaQuery.isError) {
    return null;
  }
  if (agendaQuery.isPending) {
    return <SkeletonList rowCount={loadingRowCount} />;
  }
  const instructorName = firstNameOf(instructorFullName);
  const slots = agendaQuery.data;
  if (slots.length === 0) {
    return (
      <OutlinedBlock className="bg-surface">
        <AppText variant="caption" tone="muted">
          {translate("privateLessons.agenda.freeDay", { name: instructorName })}
        </AppText>
      </OutlinedBlock>
    );
  }
  const instructorDay = planInstructorDay(slots, durationMinutes, startTime);
  return (
    <OutlinedBlock className="gap-3 bg-surface">
      <View className="flex-row items-start justify-between gap-2">
        <AppText variant="bodyStrong" className="flex-1">
          {translate("privateLessons.agenda.title", {
            name: instructorName,
            date: formatLongDate(date),
          })}
        </AppText>
        <StatusPill
          tone="neutral"
          isSmall
          label={translateCount("privateLessons.agenda.classCount", slots.length)}
        />
      </View>
      <DayTimeline
        startMinutes={instructorDay.visibleStartMinutes}
        endMinutes={instructorDay.visibleEndMinutes}
        periods={timelinePeriods(instructorDay)}
        accessibilityLabel={translate("privateLessons.agenda.timelineLabel", {
          classes: instructorDay.busyPeriods
            .map((busyPeriod) =>
              translate("privateLessons.agenda.timelineClass", {
                start: formatMinutesOfDay(busyPeriod.startMinutes),
                end: formatMinutesOfDay(busyPeriod.endMinutes),
              }),
            )
            .join(", "),
        })}
        legendLabels={{
          busy: translate("privateLessons.agenda.busy"),
          free: translate("privateLessons.agenda.free"),
          chosen: translate("privateLessons.agenda.thisLesson"),
          conflict: translate("privateLessons.agenda.thisLesson"),
        }}
      />
      {durationMinutes === null ? null : (
        <View className="gap-2">
          <AppText variant="label" tone="muted">
            {translate("privateLessons.agenda.freeFor", { minutes: durationMinutes })}
          </AppText>
          {instructorDay.freeGaps.length === 0 ? (
            <AppText variant="caption" tone="subtle">
              {translate("privateLessons.agenda.noGaps", { minutes: durationMinutes })}
            </AppText>
          ) : (
            <View className="flex-row flex-wrap gap-2">
              {instructorDay.freeGaps.map((gap) => (
                <Chip
                  key={gap.startMinutes}
                  label={gapLabel(gap)}
                  isSelected={isLessonInside(instructorDay, gap)}
                  onPress={() => onPickStartTime(formatMinutesOfDay(gap.suggestedStartMinutes))}
                />
              ))}
            </View>
          )}
        </View>
      )}
      <LessonStatus instructorDay={instructorDay} />
      <View>
        <TextLink
          label={
            areClassesShown
              ? translate("privateLessons.agenda.hideClasses")
              : translateCount("privateLessons.agenda.showClasses", slots.length)
          }
          trailingIcon={areClassesShown ? "collapse" : "expand"}
          isExpanded={areClassesShown}
          onPress={() => setAreClassesShown(!areClassesShown)}
        />
        <Collapsible isExpanded={areClassesShown}>
          {instructorDay.busyPeriods.map((busyPeriod) => (
            <View
              key={`${busyPeriod.startMinutes}-${busyPeriod.slot.kind}`}
              className="flex-row items-center gap-3 border-t border-border-subtle py-2"
            >
              <AppText variant="bodyStrong" className="w-28">
                {periodRange(busyPeriod)}
              </AppText>
              <AppText variant="caption" tone="muted" className="flex-1">
                {slotName(busyPeriod.slot, translate("privateLessons.agenda.privateLessonRow"))}
              </AppText>
            </View>
          ))}
        </Collapsible>
      </View>
    </OutlinedBlock>
  );
}
