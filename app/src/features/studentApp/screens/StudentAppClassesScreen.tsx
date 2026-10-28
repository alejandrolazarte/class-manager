import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { MakeupSection } from "@/features/studentApp/components/MakeupSection";
import { PackClassSection } from "@/features/studentApp/components/PackClassSection";
import { PastClassRow } from "@/features/studentApp/components/PastClassRow";
import { ScheduledClassRow } from "@/features/studentApp/components/ScheduledClassRow";
import { StudentChips } from "@/features/studentApp/components/StudentChips";
import { useSelectedStudent } from "@/features/studentApp/AccountStudentProvider";
import {
  agendaMarkerOf,
  heldAndAttendedCount,
  historyMonthsOf,
  isCancelledDay,
  isFirstAgendaPage,
  isLastAgendaPage,
  pastClassesOn,
} from "@/features/studentApp/studentAppAgenda";
import { classesOn, monthTitle, shortDayLabel } from "@/features/studentApp/studentAppSchedule";
import { useStudentAppClassHistory } from "@/features/studentApp/useStudentAppClassHistory";
import { useStudentAppHome } from "@/features/studentApp/useStudentAppHome";
import { agendaHeadingOf } from "@/features/home/agenda";
import { useRefetchOnFocus } from "@/hooks/useRefetchOnFocus";
import { addDays, addMonthsToDate, monthOfDate, todayIsoDate, weekOf } from "@/dates/dates";
import { translate, translateCount } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AgendaCalendar, AgendaView } from "@/ui/AgendaCalendar";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { EmptyState } from "@/ui/EmptyState";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { TonalButton } from "@/ui/TonalButton";
import { AnimatedListItem } from "@/ui/motion/AnimatedListItem";
import { SkeletonList } from "@/ui/motion/SkeletonList";

const daysPerWeek = 7;

export function StudentAppClassesScreen() {
  const router = useRouter();
  const today = todayIsoDate();
  const { data: home, isPending, isError, refetch } = useStudentAppHome();
  useRefetchOnFocus(refetch);
  const { student, selectStudent } = useSelectedStudent(home?.students ?? []);
  const [selectedDate, setSelectedDate] = useState(today);
  const [view, setView] = useState<AgendaView>("day");
  const history = useStudentAppClassHistory(
    student?.id ?? null,
    historyMonthsOf(view, selectedDate, today),
  );

  const header = (
    <ScreenHeader eyebrow={monthTitle(selectedDate)} title={translate("student.tabs.classes")} />
  );

  if (isPending) {
    return (
      <ScrollScreen header={header}>
        <SkeletonList />
      </ScrollScreen>
    );
  }
  if (isError || home === undefined) {
    return (
      <ScrollScreen header={header}>
        <Banner message={translate("student.loadError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => refetch()}
          />
        </Banner>
      </ScrollScreen>
    );
  }
  if (student === null) {
    return (
      <ScrollScreen header={header}>
        <AppText variant="body" tone="muted">
          {translate("student.students.empty")}
        </AppText>
      </ScrollScreen>
    );
  }

  const isMonthView = view === "month";
  const isPastDay = selectedDate < today;
  const step = (direction: number) =>
    setSelectedDate(
      isMonthView
        ? addMonthsToDate(selectedDate, direction)
        : addDays(selectedDate, direction * daysPerWeek),
    );
  const heading = agendaHeadingOf(view, selectedDate, today);
  const dayClasses = classesOn(student, selectedDate);
  const dayPastClasses = pastClassesOn(history.pastClasses, selectedDate);
  const dayClassCount = isPastDay ? dayPastClasses.length : dayClasses.length;
  const monthCount = heldAndAttendedCount(history.pastClasses, monthOfDate(selectedDate));
  const daySummary =
    dayClassCount === 0
      ? translate("sessions.day.noClasses")
      : translateCount("sessions.day.classCount", dayClassCount);
  const summary =
    isMonthView && monthCount.held > 0
      ? translateCount("student.history.monthSummary", monthCount.held, {
          attended: monthCount.attended,
        })
      : daySummary;
  const isLoadingDay = isPastDay && history.isPending;
  const isEmptyDay = !isLoadingDay && dayClassCount === 0 && !(isPastDay && history.isError);

  return (
    <ScrollScreen header={header}>
      <StudentChips
        students={home.students}
        selectedStudentId={student.id}
        onSelect={selectStudent}
      />
      <View className="gap-3">
        <AgendaCalendar
          view={view}
          onChangeView={setView}
          eyebrow={heading.eyebrow}
          title={heading.title}
          selectedDate={selectedDate}
          today={today}
          weekDayOf={(isoDate) => ({
            marker: agendaMarkerOf(isoDate, today, student, history.pastClasses),
            accessibilityLabel: shortDayLabel(isoDate),
          })}
          monthDayOf={(isoDate) => ({
            marker: agendaMarkerOf(isoDate, today, student, history.pastClasses),
            isCancelled: isCancelledDay(isoDate, today, student, history.pastClasses),
            accessibilityLabel: shortDayLabel(isoDate),
          })}
          onSelectDate={setSelectedDate}
          onPrevious={() => step(-1)}
          onNext={() => step(1)}
          isPreviousDisabled={isFirstAgendaPage(view, selectedDate, today)}
          isNextDisabled={isLastAgendaPage(view, selectedDate, today)}
        />
        <View className="h-7 flex-row items-center justify-between">
          <AppText variant="caption" tone="subtle">
            {history.isPending && (isPastDay || isMonthView) ? "" : summary}
          </AppText>
          <TonalButton
            label={translate("home.backToToday")}
            accessibilityLabel={translate("sessions.day.backToToday")}
            icon="today"
            isHidden={selectedDate === today}
            onPress={() => setSelectedDate(today)}
          />
        </View>
      </View>
      <View>
        {isLoadingDay ? <SkeletonList /> : null}
        {isPastDay && history.isError ? (
          <Banner message={translate("student.loadError")}>
            <Button
              variant="secondary"
              size="medium"
              label={translate("common.retry")}
              onPress={history.refetch}
            />
          </Banner>
        ) : null}
        {isEmptyDay ? (
          <EmptyState
            icon="noClasses"
            iconTone="disabled-foreground"
            message={translate("student.schedule.empty")}
          />
        ) : null}
        {isPastDay ? (
          <View key={`${student.id}-${selectedDate}`}>
            {dayPastClasses.map((pastClass, index) => (
              <AnimatedListItem
                key={`${pastClass.date}-${pastClass.startTime}-${pastClass.name}`}
                index={index}
              >
                <PastClassRow pastClass={pastClass} />
              </AnimatedListItem>
            ))}
          </View>
        ) : (
          <View key={`${student.id}-${selectedDate}`}>
            {dayClasses.map((scheduledClass, index) => (
              <AnimatedListItem
                key={`${scheduledClass.date}-${scheduledClass.startTime}-${scheduledClass.name}`}
                index={index}
              >
                <ScheduledClassRow
                  studentId={student.id}
                  scheduledClass={scheduledClass}
                  isToday={selectedDate === today}
                  onOpen={
                    scheduledClass.classGroupId === null
                      ? undefined
                      : () =>
                          router.push(
                            routes.studentAppClass({
                              classGroupId: scheduledClass.classGroupId ?? "",
                              date: scheduledClass.date,
                              startTime: scheduledClass.startTime,
                            }),
                          )
                  }
                />
              </AnimatedListItem>
            ))}
          </View>
        )}
      </View>
      <PackClassSection studentId={student.id} days={weekOf(selectedDate)} />
      <MakeupSection studentId={student.id} days={weekOf(selectedDate)} />
    </ScrollScreen>
  );
}
