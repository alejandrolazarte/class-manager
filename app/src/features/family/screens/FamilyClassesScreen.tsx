import { useState } from "react";
import { View } from "react-native";
import { ScheduledClassCard } from "@/features/family/components/ScheduledClassCard";
import { StudentChips } from "@/features/family/components/StudentChips";
import { WeekStrip } from "@/features/family/components/WeekStrip";
import { useSelectedStudent } from "@/features/family/FamilyStudentProvider";
import { classesOn, monthTitle, shortDayLabel } from "@/features/family/familySchedule";
import { useFamilyHome } from "@/features/family/useFamilyHome";
import { useRefetchOnFocus } from "@/hooks/useRefetchOnFocus";
import { addDays, todayIsoDate, weekOf } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { EmptyState } from "@/ui/EmptyState";
import { IconButton } from "@/ui/IconButton";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { Spinner } from "@/ui/Spinner";

const daysPerWeek = 7;
const lastWeekOffset = 2;

export function FamilyClassesScreen() {
  const today = todayIsoDate();
  const { data: home, isPending, isError, refetch } = useFamilyHome();
  useRefetchOnFocus(refetch);
  const { student, selectStudent } = useSelectedStudent(home?.students ?? []);
  const [weekOffset, setWeekOffset] = useState(0);
  const [selectedDate, setSelectedDate] = useState(today);

  const days = weekOf(addDays(today, weekOffset * daysPerWeek));
  const changeWeek = (offset: number) => {
    setWeekOffset(offset);
    const firstDay = weekOf(addDays(today, offset * daysPerWeek))[0] ?? today;
    setSelectedDate(offset === 0 ? today : firstDay);
  };

  const header = (
    <ScreenHeader
      eyebrow={monthTitle(selectedDate)}
      title={translate("family.tabs.classes")}
      accessory={
        <View className="flex-row">
          <IconButton
            icon="previous"
            accessibilityLabel={translate("family.schedule.previousWeek")}
            disabled={weekOffset === 0}
            onPress={() => changeWeek(weekOffset - 1)}
          />
          <IconButton
            icon="next"
            accessibilityLabel={translate("family.schedule.nextWeek")}
            disabled={weekOffset === lastWeekOffset}
            onPress={() => changeWeek(weekOffset + 1)}
          />
        </View>
      }
    />
  );

  if (isPending) {
    return (
      <ScrollScreen header={header}>
        <Spinner className="mt-6" />
      </ScrollScreen>
    );
  }
  if (isError || home === undefined) {
    return (
      <ScrollScreen header={header}>
        <Banner message={translate("family.loadError")}>
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
          {translate("family.students.empty")}
        </AppText>
      </ScrollScreen>
    );
  }

  const dayClasses = classesOn(student, selectedDate);
  return (
    <ScrollScreen header={header}>
      <StudentChips
        students={home.students}
        selectedStudentId={student.id}
        onSelect={selectStudent}
      />
      <WeekStrip
        days={days}
        today={today}
        selectedDate={selectedDate}
        hasClassesOn={(isoDate) => classesOn(student, isoDate).length > 0}
        onSelect={setSelectedDate}
      />
      <SectionTitle
        title={
          selectedDate === today
            ? translate("family.day.todayWithDate", { day: shortDayLabel(selectedDate) })
            : shortDayLabel(selectedDate)
        }
      />
      {dayClasses.length === 0 ? (
        <Card>
          <EmptyState
            icon="noClasses"
            iconTone="disabled-foreground"
            message={translate("family.schedule.empty")}
          />
        </Card>
      ) : (
        dayClasses.map((scheduledClass) => (
          <ScheduledClassCard
            key={`${scheduledClass.date}-${scheduledClass.startTime}-${scheduledClass.name}`}
            studentId={student.id}
            scheduledClass={scheduledClass}
            isToday={selectedDate === today}
          />
        ))
      )}
    </ScrollScreen>
  );
}
