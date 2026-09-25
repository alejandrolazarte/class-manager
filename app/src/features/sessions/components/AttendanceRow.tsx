import { Text, View } from "react-native";
import { AttendanceStatus, SessionStudent } from "@/features/sessions/types";
import { normalizeStudentName } from "@/features/students/studentSchema";
import { translate } from "@/i18n/translate";
import { Chip } from "@/ui/Chip";

interface AttendanceRowProps {
  student: SessionStudent;
  status: AttendanceStatus | null;
  disabled: boolean;
  onChangeStatus: (status: AttendanceStatus | null) => void;
}

export function AttendanceRow({ student, status, disabled, onChangeStatus }: AttendanceRowProps) {
  const isOwnClient =
    normalizeStudentName(student.studentFullName) === normalizeStudentName(student.clientFullName);
  const toggle = (selectedStatus: AttendanceStatus) =>
    onChangeStatus(status === selectedStatus ? null : selectedStatus);
  return (
    <View className="flex-row items-center gap-3 border-b border-gray-100 bg-white px-4 py-3">
      <View className="flex-1 gap-1">
        <Text className="text-base font-semibold text-gray-900">{student.studentFullName}</Text>
        {isOwnClient ? null : (
          <Text className="text-sm text-gray-600">
            {translate("students.list.responsible", { name: student.clientFullName })}
          </Text>
        )}
      </View>
      <Chip
        label={translate("sessions.attendance.present")}
        accessibilityLabel={translate("sessions.attendance.presentFor", {
          name: student.studentFullName,
        })}
        isSelected={status === "Present"}
        disabled={disabled}
        onPress={() => toggle("Present")}
      />
      <Chip
        label={translate("sessions.attendance.absent")}
        accessibilityLabel={translate("sessions.attendance.absentFor", {
          name: student.studentFullName,
        })}
        isSelected={status === "Absent"}
        disabled={disabled}
        onPress={() => toggle("Absent")}
      />
    </View>
  );
}
