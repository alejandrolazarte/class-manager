import { useRouter } from "expo-router";
import { InstructorListItem } from "@/features/instructors/components/InstructorListItem";
import { useInstructorsIncludingInactive } from "@/features/instructors/useInstructorsIncludingInactive";
import { SettingsListScreenLayout } from "@/features/settings/components/SettingsListScreenLayout";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";

export function InstructorListScreen() {
  const router = useRouter();
  const {
    data: instructors = [],
    isPending,
    isError,
    isRefetching,
    refetch,
  } = useInstructorsIncludingInactive();
  return (
    <SettingsListScreenLayout
      items={instructors}
      isPending={isPending}
      isError={isError}
      isRefetching={isRefetching}
      onRefetch={() => refetch()}
      keyExtractor={(instructor) => instructor.id}
      renderItem={(instructor) => (
        <InstructorListItem
          instructor={instructor}
          onPress={() => router.push(routes.instructor(instructor.id))}
        />
      )}
      emptyMessage={translate("instructors.list.empty")}
      newItemLabel={translate("instructors.list.newInstructor")}
      onNewItem={() => router.push(routes.newInstructor)}
    />
  );
}
