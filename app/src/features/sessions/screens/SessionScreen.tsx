import { useState } from "react";
import { Pressable, View } from "react-native";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { AttendanceRow } from "@/features/sessions/components/AttendanceRow";
import { CancelSessionPanel } from "@/features/sessions/components/CancelSessionPanel";
import { FeedbackSheet } from "@/features/sessions/components/FeedbackSheet";
import { ReschedulePanel } from "@/features/sessions/components/ReschedulePanel";
import { ClassDeliveriesCard } from "@/features/orders/components/ClassDeliveriesCard";
import { SubstitutePanel } from "@/features/sessions/components/SubstitutePanel";
import { formatLongDate } from "@/features/sessions/dates";
import { AttendanceStatus } from "@/features/sessions/types";
import { useSessionDetails } from "@/features/sessions/useSessionDetails";
import {
  useRecordAttendance,
  useRecordFeedback,
  useRemoveSubstitute,
  useRestoreSession,
  useRestoreSessionSchedule,
} from "@/features/sessions/useSessionMutations";
import { translate, translateCount } from "@/i18n/translate";
import { AppText, TextTone } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
import { ListDivider } from "@/ui/ListRow";
import { Screen, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { Spinner } from "@/ui/Spinner";
import { useToast } from "@/ui/ToastProvider";

interface SessionScreenProps {
  classGroupId: string;
  sessionDate: string;
}

type PendingStatuses = Record<string, AttendanceStatus | null>;

interface CountTileProps {
  count: number;
  label: string;
  className: string;
  countTone: TextTone;
}

const summarySeparator = " · ";

function CountTile({ count, label, className, countTone }: CountTileProps) {
  return (
    <View className={`flex-1 gap-0.5 rounded-2xl px-3 py-2.5 ${className}`}>
      <AppText variant="headline" tone={countTone}>
        {count}
      </AppText>
      <AppText variant="footnote" tone="muted" className="font-label">
        {label}
      </AppText>
    </View>
  );
}

export function SessionScreen({ classGroupId, sessionDate }: SessionScreenProps) {
  const { showToast } = useToast();
  const canRecordAttendance = useCan(
    permissions.attendanceRecordAll,
    permissions.attendanceRecordOwn,
  );
  const canManageSessions = useCan(permissions.sessionsManage);
  const {
    data: session,
    isPending,
    isError,
    refetch,
  } = useSessionDetails(classGroupId, sessionDate);
  const recordAttendanceMutation = useRecordAttendance(classGroupId, sessionDate);
  const restoreSessionMutation = useRestoreSession(classGroupId, sessionDate);
  const restoreSessionScheduleMutation = useRestoreSessionSchedule(classGroupId, sessionDate);
  const removeSubstituteMutation = useRemoveSubstitute(classGroupId, sessionDate);
  const [pendingStatuses, setPendingStatuses] = useState<PendingStatuses>({});
  const [hasSaveFailed, setHasSaveFailed] = useState(false);
  const recordFeedbackMutation = useRecordFeedback(classGroupId, sessionDate);
  const [feedbackStudentId, setFeedbackStudentId] = useState<string | null>(null);

  if (isPending) {
    return (
      <Screen header={<ScreenHeader navigation="back" title="" />}>
        <Spinner className="mt-6" />
      </Screen>
    );
  }
  if (isError || session === undefined) {
    return (
      <ScrollScreen
        header={<ScreenHeader navigation="back" title={translate("sessions.session.title")} />}
      >
        <Banner message={translate("common.unexpectedError")}>
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
  const canMarkAttendance =
    canRecordAttendance && session.canTakeAttendance && !session.isCancelled;

  const markAllPresent = () =>
    Promise.all(
      session.students
        .filter(
          (student) =>
            statusOf(student.studentId, student.status) === null && !student.absenceNotified,
        )
        .map((student) => changeStatus(student.studentId, "Present")),
    );

  const restoreSession = async () => {
    await restoreSessionMutation.mutateAsync();
    showToast(translate("sessions.session.restoredToast"));
  };

  const removeSubstitute = async () => {
    await removeSubstituteMutation.mutateAsync();
    showToast(translate("sessions.substitute.removed"));
  };

  const feedbackStudent = session.students.find(
    (student) => student.studentId === feedbackStudentId,
  );
  const saveFeedback = async (text: string | null) => {
    if (feedbackStudent === undefined) {
      return;
    }
    try {
      await recordFeedbackMutation.mutateAsync({ studentId: feedbackStudent.studentId, text });
      setFeedbackStudentId(null);
      showToast(translate(text === null ? "sessions.feedback.removed" : "sessions.feedback.saved"));
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="back"
          eyebrow={translate("sessions.session.title")}
          title={session.classGroupName}
          subtitle={[
            formatLongDate(session.date),
            `${session.startTime}–${session.endTime}`,
            session.instructorFullName,
          ].join(summarySeparator)}
        />
      }
    >
      {session.isCancelled ? null : (
        <View
          accessible
          accessibilityLabel={[
            translateCount("sessions.attendance.presentCount", presentCount),
            translateCount("sessions.attendance.absentCount", absentCount),
            translate("sessions.attendance.unmarkedCount", { count: unmarkedCount }),
          ].join(summarySeparator)}
          className="flex-row gap-2"
        >
          <CountTile
            count={presentCount}
            label={translateCount("sessions.attendance.presentLabel", presentCount)}
            className="bg-success-soft"
            countTone="success"
          />
          <CountTile
            count={absentCount}
            label={translateCount("sessions.attendance.absentLabel", absentCount)}
            className="bg-danger-soft"
            countTone="danger"
          />
          <CountTile
            count={unmarkedCount}
            label={translate("sessions.attendance.unmarkedLabel")}
            className="bg-muted"
            countTone="default"
          />
        </View>
      )}
      {session.isCancelled ? (
        <Banner
          tone="warning"
          icon="cancelled"
          message={
            session.cancellationReason
              ? translate("sessions.session.cancelled", { reason: session.cancellationReason })
              : translate("sessions.session.cancelledWithoutReason")
          }
        >
          {canManageSessions ? (
            <Button
              variant="outline"
              size="medium"
              label={translate("sessions.session.restore")}
              onPress={restoreSession}
              isLoading={restoreSessionMutation.isPending}
            />
          ) : null}
        </Banner>
      ) : null}
      {session.originalStartTime && !session.isCancelled ? (
        <Banner
          tone="warning"
          icon="schedule"
          message={translate("sessions.reschedule.notice", {
            original: session.originalStartTime,
          })}
        >
          {session.canReschedule && canManageSessions ? (
            <Button
              variant="outline"
              size="medium"
              label={translate("sessions.reschedule.restore")}
              onPress={() => restoreSessionScheduleMutation.mutate()}
              isLoading={restoreSessionScheduleMutation.isPending}
            />
          ) : null}
        </Banner>
      ) : null}
      {session.originalInstructorFullName && !session.isCancelled ? (
        <Banner
          tone="info"
          icon="substitute"
          message={translate("sessions.substitute.notice", {
            substitute: session.instructorFullName,
            original: session.originalInstructorFullName,
          })}
        >
          {canManageSessions ? (
            <Button
              variant="outline"
              size="medium"
              label={translate("sessions.substitute.remove")}
              onPress={removeSubstitute}
              isLoading={removeSubstituteMutation.isPending}
            />
          ) : null}
        </Banner>
      ) : null}
      {!session.isCancelled && !session.canTakeAttendance ? (
        <Banner tone="info" message={translate("sessions.session.notYet")} />
      ) : null}
      {hasSaveFailed ? <Banner message={translate("sessions.attendance.saveFailed")} /> : null}
      {session.isCancelled ? null : (
        <View className="gap-2">
          {canMarkAttendance && session.students.length > 0 ? (
            <View className="flex-row items-center justify-between gap-2">
              <AppText variant="caption" tone="subtle" className="flex-1">
                {translate("sessions.attendance.swipeHint")}
              </AppText>
              {unmarkedCount > 0 ? (
                <Pressable
                  accessibilityRole="button"
                  accessibilityLabel={translate("sessions.attendance.allPresent")}
                  onPress={markAllPresent}
                  className="flex-row items-center gap-1.5 rounded-full bg-success-soft px-3.5 py-2 active:opacity-80"
                >
                  <Icon name="allPresent" size="medium" tone="success-soft-foreground" />
                  <AppText variant="eyebrow" tone="successSoft">
                    {translate("sessions.attendance.allPresent")}
                  </AppText>
                </Pressable>
              ) : null}
            </View>
          ) : null}
          {session.students.length === 0 ? (
            <AppText variant="body" tone="muted" className="py-2">
              {translate("sessions.session.noStudents")}
            </AppText>
          ) : null}
          {session.students.map((student) => (
            <AttendanceRow
              key={student.studentId}
              student={student}
              status={statusOf(student.studentId, student.status)}
              disabled={!canRecordAttendance || !session.canTakeAttendance}
              onChangeStatus={(status) => changeStatus(student.studentId, status)}
              onOpenFeedback={
                canRecordAttendance && session.canTakeAttendance
                  ? () => setFeedbackStudentId(student.studentId)
                  : undefined
              }
            />
          ))}
        </View>
      )}
      {canRecordAttendance && !session.isCancelled ? (
        <ClassDeliveriesCard classGroupId={classGroupId} />
      ) : null}
      {session.isCancelled || !canManageSessions ? null : (
        <Card className="mt-1">
          {session.canReschedule ? (
            <>
              <ReschedulePanel
                classGroupId={classGroupId}
                sessionDate={sessionDate}
                currentStartTime={session.startTime}
              />
              <ListDivider />
            </>
          ) : null}
          <SubstitutePanel
            classGroupId={classGroupId}
            sessionDate={sessionDate}
            currentInstructorId={session.instructorId}
          />
          <ListDivider />
          <CancelSessionPanel
            classGroupId={classGroupId}
            sessionDate={sessionDate}
            hasAttendance={presentCount + absentCount > 0}
          />
        </Card>
      )}
      {feedbackStudent === undefined ? null : (
        <FeedbackSheet
          key={feedbackStudent.studentId}
          student={feedbackStudent}
          isSaving={recordFeedbackMutation.isPending}
          onClose={() => setFeedbackStudentId(null)}
          onSave={saveFeedback}
        />
      )}
    </ScrollScreen>
  );
}
