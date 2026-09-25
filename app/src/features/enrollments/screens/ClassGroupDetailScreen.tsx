import { useRouter } from "expo-router";
import { useState } from "react";
import { ActivityIndicator, ScrollView, Text, View } from "react-native";
import { useClassGroupsIncludingInactive } from "@/features/classGroups/useClassGroups";
import { RosterItem } from "@/features/enrollments/components/RosterItem";
import { todayIsoDate } from "@/features/enrollments/todayIsoDate";
import { RosterEntry } from "@/features/enrollments/types";
import { useClassRoster } from "@/features/enrollments/useClassRoster";
import { useEndEnrollment } from "@/features/enrollments/useEnrollmentMutations";
import { summarizeSchedule } from "@/features/enrollments/weekdaySummary";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { useToast } from "@/ui/ToastProvider";

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
    <ScrollView className="flex-1 bg-gray-50" contentContainerClassName="gap-4 pb-12">
      <View className="gap-1 bg-white p-4">
        <Text className="text-2xl font-bold text-gray-900">{classGroup.name}</Text>
        <Text className="text-base text-gray-700">{details.join(detailSeparator)}</Text>
        <Text className="text-base font-medium text-brand">
          {translate("enrollments.detail.spots", {
            enrolled: enrolledCount,
            capacity: classGroup.capacity,
          })}
        </Text>
      </View>
      {entryToUnenroll ? (
        <View className="px-4">
          <Banner
            tone="warning"
            message={translate("enrollments.detail.unenrollQuestion", {
              student: entryToUnenroll.studentFullName,
              classGroup: classGroup.name,
            })}
          >
            <Button
              variant="danger"
              label={translate("enrollments.detail.confirmUnenroll")}
              onPress={confirmUnenroll}
              isLoading={endEnrollmentMutation.isPending}
            />
            <Button
              variant="secondary"
              label={translate("common.cancel")}
              onPress={() => setEntryToUnenroll(null)}
            />
          </Banner>
        </View>
      ) : null}
      {hasUnenrollFailed ? (
        <View className="px-4">
          <Banner message={translate("common.unexpectedError")} />
        </View>
      ) : null}
      <View className="gap-3 px-4">
        <Button
          label={translate(
            isFull ? "enrollments.detail.classFull" : "enrollments.detail.enrollStudent",
          )}
          disabled={isFull}
          onPress={() => router.push(routes.enrollStudent(classGroup.id))}
        />
        <Button
          variant="secondary"
          label={translate("enrollments.detail.editClass")}
          onPress={() => router.push(routes.editClassGroup(classGroup.id))}
        />
      </View>
      <View>
        <Text className="px-4 pb-2 text-lg font-semibold text-gray-900">
          {translate("enrollments.detail.students")}
        </Text>
        {rosterQuery.isPending ? <ActivityIndicator className="mt-4" /> : null}
        {rosterQuery.data && roster.length === 0 ? (
          <Text className="px-4 text-base text-gray-600">
            {translate("enrollments.detail.noStudents")}
          </Text>
        ) : null}
        {roster.map((entry) => (
          <RosterItem
            key={entry.enrollmentId}
            entry={entry}
            today={todayIsoDate()}
            onUnenroll={setEntryToUnenroll}
          />
        ))}
      </View>
    </ScrollView>
  );
}
