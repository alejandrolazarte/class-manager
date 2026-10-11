import { View } from "react-native";
import { useStudentEnrollments } from "@/features/enrollments/useStudentEnrollments";
import { summarizeSchedule } from "@/features/enrollments/weekdaySummary";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { BulletRow } from "@/ui/BulletRow";

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
      <AppText variant="footnote" tone="subtle">
        {translate("enrollments.studentClasses.none")}
      </AppText>
    );
  }
  return (
    <View className="gap-1">
      {enrollments.map((enrollment) => (
        <BulletRow
          key={enrollment.enrollmentId}
          label={enrollment.classGroupName}
          detail={summarizeSchedule(enrollment.weekdays, enrollment.startTime)}
        />
      ))}
    </View>
  );
}
