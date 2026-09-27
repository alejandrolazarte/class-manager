import { View } from "react-native";
import { useStudentEnrollments } from "@/features/enrollments/useStudentEnrollments";
import { summarizeSchedule } from "@/features/enrollments/weekdaySummary";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";

interface StudentClassesProps {
  studentId: string;
}

export function StudentClasses({ studentId }: StudentClassesProps) {
  const { data: enrollments } = useStudentEnrollments(studentId);
  if (enrollments === undefined) {
    return null;
  }
  if (enrollments.length === 0) {
    return (
      <AppText variant="badge" tone="subtle">
        {translate("enrollments.studentClasses.none")}
      </AppText>
    );
  }
  return (
    <View className="flex-row flex-wrap gap-1.5">
      {enrollments.map((enrollment) => (
        <View
          key={enrollment.enrollmentId}
          className="flex-row items-center gap-1 rounded-full bg-primary-soft px-2.5 py-1.5"
        >
          <Icon name="brand" size="small" tone="primary-soft-foreground" />
          <AppText variant="badge" tone="primarySoft">
            {`${enrollment.classGroupName} · ${summarizeSchedule(enrollment.weekdays, enrollment.startTime)}`}
          </AppText>
        </View>
      ))}
    </View>
  );
}
