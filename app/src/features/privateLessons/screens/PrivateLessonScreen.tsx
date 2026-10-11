import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { CancelPrivateLessonPanel } from "@/features/privateLessons/components/CancelPrivateLessonPanel";
import { usePrivateLesson } from "@/features/privateLessons/usePrivateLesson";
import {
  useDeletePrivateLesson,
  useRecordPrivateLessonAttendance,
  useRestorePrivateLesson,
} from "@/features/privateLessons/usePrivateLessonMutations";
import { AttendanceRow } from "@/features/sessions/components/AttendanceRow";
import { formatLongDate } from "@/features/sessions/dates";
import { AttendanceStatus } from "@/features/sessions/types";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
import { ListDivider, ListRow } from "@/ui/ListRow";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { InsetPanel } from "@/ui/InsetPanel";
import { SectionTitle } from "@/ui/SectionTitle";
import { TonalButton } from "@/ui/TonalButton";
import { StatusPill } from "@/ui/StatusPill";
import { useToast } from "@/ui/ToastProvider";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { formatMoney } from "@/features/fees/money";

interface PrivateLessonScreenProps {
  privateLessonId: string;
}

type PendingStatuses = Record<string, AttendanceStatus | null>;

const detailSeparator = " · ";
const studentNameSeparator = ", ";

export function PrivateLessonScreen({ privateLessonId }: PrivateLessonScreenProps) {
  const router = useRouter();
  const canRecordAttendance = useCan(
    permissions.attendanceRecordAll,
    permissions.attendanceRecordOwn,
  );
  const canManageLessons = useCan(
    permissions.privateLessonsManageAll,
    permissions.privateLessonsManageOwn,
  );
  const { showToast } = useToast();
  const currencyCode = useBusinessCurrency();
  const lessonQuery = usePrivateLesson(privateLessonId);
  const recordAttendanceMutation = useRecordPrivateLessonAttendance(privateLessonId);
  const restoreMutation = useRestorePrivateLesson(privateLessonId);
  const deleteMutation = useDeletePrivateLesson(privateLessonId);
  const [pendingStatuses, setPendingStatuses] = useState<PendingStatuses>({});
  const [hasSaveFailed, setHasSaveFailed] = useState(false);
  const [isConfirmingDelete, setIsConfirmingDelete] = useState(false);

  const lesson = lessonQuery.data;
  if (lesson === undefined) {
    return (
      <SettingsItemState
        isPending={lessonQuery.isPending}
        isError={lessonQuery.isError}
        notFoundMessage={translate("privateLessons.notFound")}
        onRetry={() => lessonQuery.refetch()}
      />
    );
  }

  const statusOf = (studentId: string, savedStatus: AttendanceStatus | null) =>
    studentId in pendingStatuses ? (pendingStatuses[studentId] ?? null) : savedStatus;
  const changeStatus = async (studentId: string, status: AttendanceStatus | null) => {
    setHasSaveFailed(false);
    setPendingStatuses((current) => ({ ...current, [studentId]: status }));
    try {
      await recordAttendanceMutation.mutateAsync({ studentId, status });
    } catch {
      setHasSaveFailed(true);
    } finally {
      setPendingStatuses((current) => {
        const { [studentId]: _forgotten, ...remaining } = current;
        return remaining;
      });
    }
  };
  const hasAttendance = lesson.students.some(
    (student) => statusOf(student.studentId, student.status) !== null,
  );
  const unmarkedStudents = lesson.students.filter(
    (student) => statusOf(student.studentId, student.status) === null,
  );

  const deleteLesson = async () => {
    await deleteMutation.mutateAsync();
    showToast(translate("privateLessons.deleted"));
    router.back();
  };

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="back"
          eyebrow={translate("privateLessons.title")}
          title={lesson.students
            .map((student) => student.studentFullName)
            .join(studentNameSeparator)}
          subtitle={[
            `${formatLongDate(lesson.date)} · ${lesson.startTime}–${lesson.endTime}`,
            lesson.instructorFullName,
            lesson.location,
          ]
            .filter(Boolean)
            .join(detailSeparator)}
        />
      }
    >
      {lesson.isTrial ? (
        <StatusPill
          tone="primary"
          label={
            lesson.trialPrice === null
              ? translate("privateLessons.trialFree")
              : translate("privateLessons.trialPaid", {
                  price: formatMoney(lesson.trialPrice, currencyCode),
                })
          }
        />
      ) : null}
      {lesson.notes ? (
        <InsetPanel>
          <SectionTitle title={translate("clients.detail.notes")} isOverline />
          <AppText variant="body">{lesson.notes}</AppText>
        </InsetPanel>
      ) : null}
      {lesson.isCancelled ? (
        <Banner
          tone="warning"
          icon="cancelled"
          message={
            lesson.cancellationReason
              ? translate("sessions.session.cancelled", { reason: lesson.cancellationReason })
              : translate("sessions.session.cancelledWithoutReason")
          }
        >
          {canManageLessons ? (
            <Button
              variant="outline"
              size="medium"
              label={translate("sessions.session.restore")}
              onPress={() => restoreMutation.mutate()}
              isLoading={restoreMutation.isPending}
            />
          ) : null}
        </Banner>
      ) : null}
      {!lesson.isCancelled && !lesson.canTakeAttendance ? (
        <Banner tone="info" message={translate("sessions.session.notYet")} />
      ) : null}
      {hasSaveFailed ? <Banner message={translate("sessions.attendance.saveFailed")} /> : null}
      {lesson.isCancelled ? null : (
        <View className="gap-2">
          {lesson.canTakeAttendance && canRecordAttendance ? (
            <View className="flex-row items-center justify-between gap-2">
              <AppText variant="caption" tone="subtle" className="flex-1">
                {translate("sessions.attendance.swipeHint")}
              </AppText>
              {unmarkedStudents.length > 1 ? (
                <TonalButton
                  tone="success"
                  icon="allPresent"
                  label={translate("sessions.attendance.allPresent")}
                  onPress={() =>
                    Promise.all(
                      unmarkedStudents.map((student) => changeStatus(student.studentId, "Present")),
                    )
                  }
                />
              ) : null}
            </View>
          ) : null}
          {lesson.students.map((student) => (
            <AttendanceRow
              key={student.studentId}
              student={student}
              status={statusOf(student.studentId, student.status)}
              disabled={!canRecordAttendance || !lesson.canTakeAttendance}
              onChangeStatus={(status) => changeStatus(student.studentId, status)}
            />
          ))}
        </View>
      )}
      {isConfirmingDelete ? (
        <ConfirmDialog
          title={translate("privateLessons.deleteQuestion")}
          message={translate("privateLessons.deleteMessage")}
          confirmLabel={translate("common.confirmDelete")}
          onCancel={() => setIsConfirmingDelete(false)}
          onConfirm={deleteLesson}
          isConfirming={deleteMutation.isPending}
        />
      ) : null}
      {!canManageLessons ? null : hasAttendance ? (
        <AppText variant="caption" tone="subtle">
          {translate("privateLessons.lockedByAttendance")}
        </AppText>
      ) : (
        <View className="mt-1 border-y border-border-subtle">
          <ListRow
            icon="schedule"
            label={translate("privateLessons.move")}
            onPress={() => router.push(routes.editPrivateLesson(lesson.id))}
          />
          {lesson.isCancelled ? null : (
            <>
              <ListDivider />
              <CancelPrivateLessonPanel privateLessonId={lesson.id} />
            </>
          )}
          <ListDivider />
          <ListRow
            icon="delete"
            iconTone="danger"
            isDestructive
            label={translate("privateLessons.delete")}
            onPress={() => setIsConfirmingDelete(true)}
          />
        </View>
      )}
    </ScrollScreen>
  );
}
