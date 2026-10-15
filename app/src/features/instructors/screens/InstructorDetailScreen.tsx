import { useRouter } from "expo-router";
import { Linking, View } from "react-native";
import { InviteInstructorAppSection } from "@/features/instructors/components/InviteInstructorAppSection";
import { useInstructorsIncludingInactive } from "@/features/instructors/useInstructorsIncludingInactive";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { InactiveChip } from "@/features/settings/components/InactiveChip";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { ActionTile } from "@/ui/ActionTile";
import { Button } from "@/ui/Button";
import { Avatar } from "@/ui/Avatar";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";

const emailScheme = "mailto:";

interface InstructorDetailScreenProps {
  instructorId: string;
}

export function InstructorDetailScreen({ instructorId }: InstructorDetailScreenProps) {
  const router = useRouter();
  const instructorsQuery = useInstructorsIncludingInactive();
  const canManageInstructors = useCan(permissions.instructorsManage);
  const canManageMembers = useCan(permissions.membersManage);
  const instructor = instructorsQuery.data?.find((candidate) => candidate.id === instructorId);

  if (instructor === undefined) {
    return (
      <SettingsItemState
        isPending={instructorsQuery.isPending}
        isError={instructorsQuery.isError}
        notFoundMessage={translate("instructors.form.notFound")}
        onRetry={() => instructorsQuery.refetch()}
      />
    );
  }

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="back"
          navigationAction={
            canManageInstructors ? (
              <Button
                variant="ghost"
                size="medium"
                icon="edit"
                label={translate("instructors.detail.edit")}
                accessibilityLabel={translate("instructors.detail.editAccessibility", {
                  name: instructor.fullName,
                })}
                onPress={() => router.push(routes.editInstructor(instructor.id))}
              />
            ) : undefined
          }
          leading={<Avatar name={instructor.fullName} tone="primary" size="large" />}
          eyebrow={translate("instructors.detail.title")}
          title={instructor.fullName}
          caption={instructor.email ?? undefined}
        />
      }
    >
      {instructor.isActive ? null : (
        <View className="flex-row">
          <InactiveChip />
        </View>
      )}
      {instructor.email ? (
        <View className="flex-row">
          <ActionTile
            icon="email"
            label={translate("instructors.detail.email")}
            onPress={() => Linking.openURL(`${emailScheme}${instructor.email}`)}
          />
        </View>
      ) : null}
      {canManageMembers && instructor.isActive ? (
        <InviteInstructorAppSection instructor={instructor} />
      ) : null}
    </ScrollScreen>
  );
}
