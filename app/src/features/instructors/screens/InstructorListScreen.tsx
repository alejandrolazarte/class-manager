import { useRouter } from "expo-router";
import { InstructorListItem } from "@/features/instructors/components/InstructorListItem";
import { useInstructorsIncludingInactive } from "@/features/instructors/useInstructorsIncludingInactive";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { SettingsListScreenLayout } from "@/features/settings/components/SettingsListScreenLayout";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";

export function InstructorListScreen() {
  const router = useRouter();
  const canManageInstructors = useCan(permissions.instructorsManage);
  const {
    data: instructors = [],
    isPending,
    isError,
    isRefetching,
    refetch,
  } = useInstructorsIncludingInactive();
  return (
    <SettingsListScreenLayout
      title={translate("instructors.list.title")}
      items={instructors}
      isPending={isPending}
      isError={isError}
      isRefetching={isRefetching}
      onRefetch={() => refetch()}
      keyExtractor={(instructor) => instructor.id}
      renderItem={(instructor) => (
        <InstructorListItem
          instructor={instructor}
          onPress={
            canManageInstructors ? () => router.push(routes.instructor(instructor.id)) : undefined
          }
        />
      )}
      emptyIcon="instructors"
      emptyMessage={translate("instructors.list.empty")}
      newItemLabel={translate("instructors.list.newInstructor")}
      onNewItem={canManageInstructors ? () => router.push(routes.newInstructor) : undefined}
    />
  );
}
