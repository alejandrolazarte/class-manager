import { useState } from "react";
import { View } from "react-native";
import { ClassMaterialSection } from "@/features/studentApp/components/ClassMaterialSection";
import { MakeupSection } from "@/features/studentApp/components/MakeupSection";
import { PackClassSection } from "@/features/studentApp/components/PackClassSection";
import { ScheduledClassCard } from "@/features/studentApp/components/ScheduledClassCard";
import { StudentChips } from "@/features/studentApp/components/StudentChips";
import { WeekStrip } from "@/features/studentApp/components/WeekStrip";
import { useSelectedStudent } from "@/features/studentApp/AccountStudentProvider";
import { classesOn, monthTitle, shortDayLabel } from "@/features/studentApp/studentAppSchedule";
import { useStudentAppHome } from "@/features/studentApp/useStudentAppHome";
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

export function StudentAppClassesScreen() {
  const today = todayIsoDate();
  const { data: home, isPending, isError, refetch } = useStudentAppHome();
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
      title={translate("student.tabs.classes")}
      accessory={
        <View className="flex-row">
          <IconButton
            icon="previous"
            accessibilityLabel={translate("student.schedule.previousWeek")}
            disabled={weekOffset === 0}
            onPress={() => changeWeek(weekOffset - 1)}
          />
          <IconButton
            icon="next"
            accessibilityLabel={translate("student.schedule.nextWeek")}
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
            ? translate("student.day.todayWithDate", { day: shortDayLabel(selectedDate) })
            : shortDayLabel(selectedDate)
        }
      />
      {dayClasses.length === 0 ? (
        <Card>
          <EmptyState
            icon="noClasses"
            iconTone="disabled-foreground"
            message={translate("student.schedule.empty")}
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
      <ClassMaterialSection materials={student.materials} />
      <PackClassSection studentId={student.id} days={days} />
      <MakeupSection studentId={student.id} days={days} />
    </ScrollScreen>
  );
}
