import { useState } from "react";
import { ActivityIndicator, ScrollView, Text, View } from "react-native";
import { AttendanceRow } from "@/features/sessions/components/AttendanceRow";
import { CancelSessionPanel } from "@/features/sessions/components/CancelSessionPanel";
import { ReschedulePanel } from "@/features/sessions/components/ReschedulePanel";
import { formatLongDate } from "@/features/sessions/dates";
import { AttendanceStatus } from "@/features/sessions/types";
import { useSessionDetails } from "@/features/sessions/useSessionDetails";
import {
  useRecordAttendance,
  useRestoreSession,
  useRestoreSessionSchedule,
} from "@/features/sessions/useSessionMutations";
import { translate, translateCount } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";

interface SessionScreenProps {
  classGroupId: string;
  sessionDate: string;
}

type PendingStatuses = Record<string, AttendanceStatus | null>;

const summarySeparator = " · ";

export function SessionScreen({ classGroupId, sessionDate }: SessionScreenProps) {
  const {
    data: session,
    isPending,
    isError,
    refetch,
  } = useSessionDetails(classGroupId, sessionDate);
  const recordAttendanceMutation = useRecordAttendance(classGroupId, sessionDate);
  const restoreSessionMutation = useRestoreSession(classGroupId, sessionDate);
  const restoreSessionScheduleMutation = useRestoreSessionSchedule(classGroupId, sessionDate);
  const [pendingStatuses, setPendingStatuses] = useState<PendingStatuses>({});
  const [hasSaveFailed, setHasSaveFailed] = useState(false);

  if (isPending) {
    return <ActivityIndicator className="mt-6" />;
  }
  if (isError || session === undefined) {
    return (
      <View className="p-4">
        <Banner message={translate("common.unexpectedError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={() => refetch()} />
        </Banner>
      </View>
    );
  }

  const statusOf = (studentId: string, savedStatus: AttendanceStatus | null) =>
    studentId in pendingStatuses ? (pendingStatuses[studentId] ?? null) : savedStatus;
  const forgetPending = (studentId: string) =>
    setPendingStatuses((current) => {
      const { [studentId]: _forgotten, ...remaining } = current;
      return remaining;
    });

  const changeStatus = async (studentId: string, status: AttendanceStatus | null) => {
    setHasSaveFailed(false);
    setPendingStatuses((current) => ({ ...current, [studentId]: status }));
    try {
      await recordAttendanceMutation.mutateAsync({ studentId, status });
    } catch {
      setHasSaveFailed(true);
    } finally {
      forgetPending(studentId);
    }
  };

  const statuses = session.students.map((student) => statusOf(student.studentId, student.status));
  const presentCount = statuses.filter((status) => status === "Present").length;
  const absentCount = statuses.filter((status) => status === "Absent").length;
  const unmarkedCount = statuses.length - presentCount - absentCount;

  return (
    <ScrollView className="flex-1 bg-gray-50" contentContainerClassName="gap-4 pb-12">
      <View className="gap-1 bg-white p-4">
        <Text className="text-2xl font-bold text-gray-900">{session.classGroupName}</Text>
        <Text className="text-base text-gray-700">
          {`${formatLongDate(session.date)} · ${session.startTime}–${session.endTime}`}
        </Text>
        {session.isCancelled ? null : (
          <Text className="text-base font-medium text-brand">
            {[
              translateCount("sessions.attendance.presentCount", presentCount),
              translateCount("sessions.attendance.absentCount", absentCount),
              translate("sessions.attendance.unmarkedCount", { count: unmarkedCount }),
            ].join(summarySeparator)}
          </Text>
        )}
      </View>
      {session.isCancelled ? (
        <View className="px-4">
          <Banner
            tone="warning"
            message={
              session.cancellationReason
                ? translate("sessions.session.cancelled", { reason: session.cancellationReason })
                : translate("sessions.session.cancelledWithoutReason")
            }
          >
            <Button
              variant="secondary"
              label={translate("sessions.session.restore")}
              onPress={() => restoreSessionMutation.mutate()}
              isLoading={restoreSessionMutation.isPending}
            />
          </Banner>
        </View>
      ) : null}
      {session.originalStartTime && !session.isCancelled ? (
        <View className="px-4">
          <Banner
            tone="warning"
            message={translate("sessions.reschedule.notice", {
              original: session.originalStartTime,
            })}
          >
            {session.canReschedule ? (
              <Button
                variant="secondary"
                label={translate("sessions.reschedule.restore")}
                onPress={() => restoreSessionScheduleMutation.mutate()}
                isLoading={restoreSessionScheduleMutation.isPending}
              />
            ) : null}
          </Banner>
        </View>
      ) : null}
      {!session.isCancelled && !session.canTakeAttendance ? (
        <Text className="px-4 text-base text-gray-600">{translate("sessions.session.notYet")}</Text>
      ) : null}
      {hasSaveFailed ? (
        <View className="px-4">
          <Banner message={translate("sessions.attendance.saveFailed")} />
        </View>
      ) : null}
      {session.isCancelled ? null : (
        <View>
          {session.students.length === 0 ? (
            <Text className="px-4 text-base text-gray-600">
              {translate("sessions.session.noStudents")}
            </Text>
          ) : null}
          {session.students.map((student) => (
            <AttendanceRow
              key={student.studentId}
              student={student}
              status={statusOf(student.studentId, student.status)}
              disabled={!session.canTakeAttendance}
              onChangeStatus={(status) => changeStatus(student.studentId, status)}
            />
          ))}
        </View>
      )}
      {session.isCancelled ? null : (
        <View className="gap-3 px-4">
          {session.canReschedule ? (
            <ReschedulePanel classGroupId={classGroupId} sessionDate={sessionDate} />
          ) : null}
          <CancelSessionPanel classGroupId={classGroupId} sessionDate={sessionDate} />
        </View>
      )}
    </ScrollView>
  );
}
