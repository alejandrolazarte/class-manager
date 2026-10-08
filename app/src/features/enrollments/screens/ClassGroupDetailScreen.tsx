import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { useClassGroupsIncludingInactive } from "@/features/classGroups/useClassGroups";
import { RosterItem } from "@/features/enrollments/components/RosterItem";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { todayIsoDate } from "@/features/sessions/dates";
import { RosterEntry } from "@/features/enrollments/types";
import { useClassRoster } from "@/features/enrollments/useClassRoster";
import { useEndEnrollment } from "@/features/enrollments/useEnrollmentMutations";
import { summarizeSchedule } from "@/features/enrollments/weekdaySummary";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { translate, translateCount } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
import { useToast } from "@/ui/ToastProvider";
import { AppText } from "@/ui/AppText";
import { Spinner } from "@/ui/Spinner";
import { Card } from "@/ui/Card";
import { ProgressBar } from "@/ui/ProgressBar";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";

interface ClassGroupDetailScreenProps {
  classGroupId: string;
}

const detailSeparator = " · ";

export function ClassGroupDetailScreen({ classGroupId }: ClassGroupDetailScreenProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const classGroupsQuery = useClassGroupsIncludingInactive();
  const rosterQuery = useClassRoster(classGroupId);
  const endEnrollmentMutation = useEndEnrollment();
  const [entryToUnenroll, setEntryToUnenroll] = useState<RosterEntry | null>(null);
  const canManageClassGroups = useCan(permissions.classGroupsManage);
  const canManageEnrollments = useCan(
    permissions.enrollmentsManage,
    permissions.enrollmentsManageOwn,
  );
  const [hasUnenrollFailed, setHasUnenrollFailed] = useState(false);
  const classGroup = classGroupsQuery.data?.find((candidate) => candidate.id === classGroupId);

  if (classGroup === undefined) {
    return (
      <SettingsItemState
        isPending={classGroupsQuery.isPending}
        isError={classGroupsQuery.isError}
        notFoundMessage={translate("classGroups.form.notFound")}
        onRetry={() => classGroupsQuery.refetch()}
      />
    );
  }

  const roster = rosterQuery.data ?? [];
  const enrolledCount = rosterQuery.data ? roster.length : classGroup.enrolledCount;
  const isFull = enrolledCount >= classGroup.capacity;
  const details = [
    `${summarizeSchedule(classGroup.weekdays, classGroup.startTime)}–${classGroup.endTime}`,
    classGroup.instructorFullName,
    classGroup.location,
  ].filter((detail): detail is string => Boolean(detail));

  const confirmUnenroll = async () => {
    if (entryToUnenroll === null) {
      return;
    }
    setHasUnenrollFailed(false);
    try {
      await endEnrollmentMutation.mutateAsync(entryToUnenroll.enrollmentId);
      showToast(
        translate("enrollments.detail.unenrolled", {
          student: entryToUnenroll.studentFullName,
          classGroup: classGroup.name,
        }),
      );
      setEntryToUnenroll(null);
    } catch {
      setHasUnenrollFailed(true);
    }
  };

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="back"
          navigationAction={
            canManageClassGroups ? (
              <Button
                variant="ghost"
                size="medium"
                icon="edit"
                label={translate("enrollments.detail.editClass")}
                onPress={() => router.push(routes.editClassGroup(classGroup.id))}
              />
            ) : undefined
          }
          title={classGroup.name}
          subtitle={details.join(detailSeparator)}
        />
      }
    >
      <Card className="gap-3 p-4">
        <View className="flex-row items-baseline justify-between">
          <AppText variant="headline" tone="primary">
            {translate("enrollments.detail.spots", {
              enrolled: enrolledCount,
              capacity: classGroup.capacity,
            })}
          </AppText>
          <AppText variant="label" tone="subtle">
            {isFull
              ? translate("classGroups.card.full")
              : translateCount("enrollments.detail.free", classGroup.capacity - enrolledCount)}
          </AppText>
        </View>
        <View className="flex-row">
          <ProgressBar ratio={classGroup.capacity > 0 ? enrolledCount / classGroup.capacity : 0} />
        </View>
        {canManageEnrollments ? (
          <Button
            size="medium"
            icon="enroll"
            label={translate(
              isFull ? "enrollments.detail.classFull" : "enrollments.detail.enrollStudent",
            )}
            disabled={isFull}
            onPress={() => router.push(routes.enrollStudent(classGroup.id))}
          />
        ) : null}
      </Card>
      {entryToUnenroll ? (
        <ConfirmDialog
          title={translate("enrollments.detail.unenrollQuestion", {
            student: entryToUnenroll.studentFullName,
          })}
          message={translate("enrollments.detail.unenrollMessage", { classGroup: classGroup.name })}
          confirmLabel={translate("enrollments.detail.confirmUnenroll")}
          onCancel={() => setEntryToUnenroll(null)}
          onConfirm={confirmUnenroll}
          isConfirming={endEnrollmentMutation.isPending}
        />
      ) : null}
      {hasUnenrollFailed ? <Banner message={translate("common.unexpectedError")} /> : null}
      <SectionTitle title={translate("enrollments.detail.students")} />
      <Card>
        {rosterQuery.isPending ? <Spinner className="my-4" /> : null}
        {rosterQuery.data && roster.length === 0 ? (
          <AppText variant="body" tone="muted" className="px-4 py-[18px]">
            {translate("enrollments.detail.noStudents")}
          </AppText>
        ) : null}
        {roster.map((entry) => (
          <RosterItem
            key={entry.enrollmentId}
            entry={entry}
            today={todayIsoDate()}
            onUnenroll={canManageEnrollments ? setEntryToUnenroll : undefined}
          />
        ))}
      </Card>
    </ScrollScreen>
  );
}
