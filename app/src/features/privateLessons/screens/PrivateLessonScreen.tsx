import { useRouter } from "expo-router";
import { useState } from "react";
import { Pressable, View } from "react-native";
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
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
import { ListDivider, ListRow } from "@/ui/ListRow";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
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
        <View className="gap-0.5 rounded-2xl bg-muted px-3.5 py-3">
          <AppText variant="overline" tone="subtle">
            {translate("clients.detail.notes")}
          </AppText>
          <AppText variant="body">{lesson.notes}</AppText>
        </View>
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
          <Button
            variant="outline"
            size="medium"
            label={translate("sessions.session.restore")}
            onPress={() => restoreMutation.mutate()}
            isLoading={restoreMutation.isPending}
          />
        </Banner>
      ) : null}
      {!lesson.isCancelled && !lesson.canTakeAttendance ? (
        <Banner tone="info" message={translate("sessions.session.notYet")} />
      ) : null}
      {hasSaveFailed ? <Banner message={translate("sessions.attendance.saveFailed")} /> : null}
      {lesson.isCancelled ? null : (
        <View className="gap-2">
          {lesson.canTakeAttendance ? (
            <View className="flex-row items-center justify-between gap-2">
              <AppText variant="caption" tone="subtle" className="flex-1">
                {translate("sessions.attendance.swipeHint")}
              </AppText>
              {unmarkedStudents.length > 1 ? (
                <Pressable
                  accessibilityRole="button"
                  accessibilityLabel={translate("sessions.attendance.allPresent")}
                  onPress={() =>
                    Promise.all(
                      unmarkedStudents.map((student) => changeStatus(student.studentId, "Present")),
                    )
                  }
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
          {lesson.students.map((student) => (
            <AttendanceRow
              key={student.studentId}
              student={student}
              status={statusOf(student.studentId, student.status)}
              disabled={!lesson.canTakeAttendance}
              onChangeStatus={(status) => changeStatus(student.studentId, status)}
            />
          ))}
        </View>
      )}
      {isConfirmingDelete ? (
        <Banner tone="warning" icon="delete" message={translate("privateLessons.deleteQuestion")}>
          <View className="flex-row gap-2">
            <View className="flex-1">
              <Button
                variant="secondary"
                size="medium"
                label={translate("common.cancel")}
                onPress={() => setIsConfirmingDelete(false)}
              />
            </View>
            <View className="flex-1">
              <Button
                variant="danger"
                size="medium"
                label={translate("privateLessons.confirmDelete")}
                onPress={deleteLesson}
                isLoading={deleteMutation.isPending}
              />
            </View>
          </View>
        </Banner>
      ) : null}
      {hasAttendance ? (
        <AppText variant="caption" tone="subtle">
          {translate("privateLessons.lockedByAttendance")}
        </AppText>
      ) : (
        <Card className="mt-1">
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
            label={translate("privateLessons.delete")}
            onPress={() => setIsConfirmingDelete(true)}
          />
        </Card>
      )}
    </ScrollScreen>
  );
}
