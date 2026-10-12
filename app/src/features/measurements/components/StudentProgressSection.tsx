import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { ClientDetails } from "@/features/clients/types";
import { StudentMeasurementRow } from "@/features/measurements/components/StudentMeasurementRow";
import { useStudentMeasurements } from "@/features/measurements/useMeasurements";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { StudentChips } from "@/features/studentApp/components/StudentChips";
import { translate } from "@/i18n/translate";
import { ClientTab, routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { EmptyState } from "@/ui/EmptyState";
import { SkeletonList } from "@/ui/motion/SkeletonList";

interface StudentProgressSectionProps {
  client: ClientDetails;
  clientTab: ClientTab;
}

export function StudentProgressSection({ client, clientTab }: StudentProgressSectionProps) {
  const router = useRouter();
  const canRecord = useCan(permissions.attendanceRecordAll, permissions.attendanceRecordOwn);
  const [selectedStudentId, setSelectedStudentId] = useState(client.students[0]?.id ?? "");
  const studentId = client.students.some((student) => student.id === selectedStudentId)
    ? selectedStudentId
    : (client.students[0]?.id ?? "");
  const {
    data: measurements = [],
    isPending,
    isError,
    refetch,
  } = useStudentMeasurements(studentId);

  if (studentId.length === 0) {
    return null;
  }

  return (
    <View className="gap-2">
      <StudentChips
        students={client.students}
        selectedStudentId={studentId}
        onSelect={setSelectedStudentId}
      />
      {isPending ? <SkeletonList /> : null}
      {isError ? (
        <Banner message={translate("common.unexpectedError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => refetch()}
          />
        </Banner>
      ) : null}
      {!isPending && !isError && measurements.length === 0 ? (
        <EmptyState icon="trendingUp" message={translate("measurements.student.noMeasurements")} />
      ) : null}
      <View>
        {measurements.map((studentMeasurement) => (
          <StudentMeasurementRow
            key={studentMeasurement.measurement.id}
            studentMeasurement={studentMeasurement}
            onPress={() =>
              router.push(
                routes.studentMeasurement(
                  clientTab,
                  client.id,
                  studentId,
                  studentMeasurement.measurement.id,
                ),
              )
            }
          />
        ))}
      </View>
      {canRecord && measurements.length > 0 ? (
        <Button
          size="medium"
          icon="add"
          label={translate("measurements.record")}
          onPress={() => router.push(routes.recordMeasurement(clientTab, client.id, studentId))}
        />
      ) : null}
    </View>
  );
}
