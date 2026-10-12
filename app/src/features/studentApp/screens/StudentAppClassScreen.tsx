import { useLocalSearchParams } from "expo-router";
import { ClassMaterialList } from "@/features/studentApp/components/ClassMaterialList";
import { ScheduledClassRow } from "@/features/studentApp/components/ScheduledClassRow";
import { useSelectedStudent } from "@/features/studentApp/AccountStudentProvider";
import { materialOf, shortDayLabel } from "@/features/studentApp/studentAppSchedule";
import { useStudentAppHome } from "@/features/studentApp/useStudentAppHome";
import { todayIsoDate } from "@/features/sessions/dates";
import { useRefetchOnFocus } from "@/hooks/useRefetchOnFocus";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SkeletonDetail } from "@/ui/motion/SkeletonDetail";

interface StudentAppClassParameters extends Record<string, string> {
  classGroupId: string;
  date: string;
  startTime: string;
}

export function StudentAppClassScreen() {
  const { classGroupId, date, startTime } = useLocalSearchParams<StudentAppClassParameters>();
  const today = todayIsoDate();
  const { data: home, isPending, isError, refetch } = useStudentAppHome();
  useRefetchOnFocus(refetch);
  const { student } = useSelectedStudent(home?.students ?? []);
  const material = student === null ? null : materialOf(student, classGroupId);
  const scheduledClass =
    student?.nextClasses.find(
      (nextClass) =>
        nextClass.classGroupId === classGroupId &&
        nextClass.date === date &&
        nextClass.startTime === startTime,
    ) ?? null;

  const header = (
    <ScreenHeader
      navigation="back"
      eyebrow={
        date === today
          ? translate("student.day.todayWithDate", { day: shortDayLabel(date) })
          : shortDayLabel(date)
      }
      title={scheduledClass?.name ?? material?.className ?? translate("student.tabs.classes")}
    />
  );

  if (isPending) {
    return (
      <ScrollScreen header={header}>
        <SkeletonDetail />
      </ScrollScreen>
    );
  }
  if (isError || student === null) {
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

  return (
    <ScrollScreen header={header}>
      {scheduledClass === null ? null : (
        <ScheduledClassRow
          studentId={student.id}
          scheduledClass={scheduledClass}
          isToday={date === today}
        />
      )}
      {material === null ? null : <ClassMaterialList material={material} />}
    </ScrollScreen>
  );
}
