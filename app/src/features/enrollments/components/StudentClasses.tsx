import { View } from "react-native";
import { useStudentEnrollments } from "@/features/enrollments/useStudentEnrollments";
import { summarizeSchedule } from "@/features/enrollments/weekdaySummary";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";

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
      <AppText variant="caption" tone="subtle">
        {translate("enrollments.studentClasses.none")}
      </AppText>
    );
  }
  return (
    <View className="gap-1">
      {enrollments.map((enrollment) => (
        <AppText variant="caption" tone="primary" key={enrollment.enrollmentId}>
          {`${enrollment.classGroupName} · ${summarizeSchedule(enrollment.weekdays, enrollment.startTime)}`}
        </AppText>
      ))}
    </View>
  );
}
