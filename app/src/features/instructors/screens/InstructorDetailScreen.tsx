import { useRouter } from "expo-router";
import { Linking, Pressable, View } from "react-native";
import { InviteInstructorAppSection } from "@/features/instructors/components/InviteInstructorAppSection";
import { useInstructorsIncludingInactive } from "@/features/instructors/useInstructorsIncludingInactive";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { InactiveChip } from "@/features/settings/components/InactiveChip";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Icon } from "@/ui/Icon";
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
              <Pressable
                accessibilityRole="button"
                accessibilityLabel={translate("instructors.detail.editAccessibility", {
                  name: instructor.fullName,
                })}
                onPress={() => router.push(routes.editInstructor(instructor.id))}
                className="flex-row items-center gap-1 rounded-full px-3 py-2.5 active:bg-muted"
              >
                <Icon name="edit" size="medium" tone="primary" />
                <AppText variant="bodyStrong" tone="primary">
                  {translate("instructors.detail.edit")}
                </AppText>
              </Pressable>
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
        <Pressable
          accessibilityRole="button"
          accessibilityLabel={translate("instructors.detail.email")}
          onPress={() => Linking.openURL(`${emailScheme}${instructor.email}`)}
          className="min-h-[60px] items-center justify-center gap-1 rounded-2xl border-[1.5px] border-border bg-surface px-2 py-2.5 active:bg-muted"
        >
          <Icon name="email" size="medium" tone="primary" />
          <AppText variant="link" numberOfLines={1}>
            {translate("instructors.detail.email")}
          </AppText>
        </Pressable>
      ) : null}
      {canManageMembers && instructor.isActive ? (
        <InviteInstructorAppSection instructor={instructor} />
      ) : null}
    </ScrollScreen>
  );
}
