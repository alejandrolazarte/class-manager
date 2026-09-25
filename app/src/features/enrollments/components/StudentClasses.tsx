import { Text, View } from "react-native";
import { useStudentEnrollments } from "@/features/enrollments/useStudentEnrollments";
import { summarizeSchedule } from "@/features/enrollments/weekdaySummary";
import { translate } from "@/i18n/translate";

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
      <Text className="text-sm text-gray-500">{translate("enrollments.studentClasses.none")}</Text>
    );
  }
  return (
    <View className="gap-1">
      {enrollments.map((enrollment) => (
        <Text key={enrollment.enrollmentId} className="text-sm text-brand">
          {`${enrollment.classGroupName} · ${summarizeSchedule(enrollment.weekdays, enrollment.startTime)}`}
        </Text>
      ))}
    </View>
  );
}
