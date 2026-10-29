import { View } from "react-native";
import { StudentAppFamilyMember } from "@/features/studentApp/types";
import { useStudentAppFamily } from "@/features/studentApp/useStudentAppFamily";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { HairlineRow } from "@/ui/HairlineRow";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { StatusPill } from "@/ui/StatusPill";
import { SkeletonList } from "@/ui/motion/SkeletonList";

function FamilyMemberRow({
  member,
  hasDivider,
}: {
  member: StudentAppFamilyMember;
  hasDivider: boolean;
}) {
  const fullName = member.isSignedIn
    ? translate("student.family.you", { name: member.fullName })
    : member.fullName;
  return (
    <HairlineRow accessibilityLabel={fullName} hasDivider={hasDivider}>
      <Avatar name={member.fullName} size="small" photoUrl={member.photoUrl} />
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong" numberOfLines={1}>
          {fullName}
        </AppText>
        {member.email === null ? null : (
          <AppText variant="caption" tone="muted" numberOfLines={1}>
            {member.email}
          </AppText>
        )}
      </View>
      {member.paysForFamily ? (
        <StatusPill label={translate("student.family.responsible")} tone="primary" isSmall />
      ) : null}
    </HairlineRow>
  );
}

export function StudentAppFamilyScreen() {
  const { data: family, isPending, isError, refetch } = useStudentAppFamily();
  const header = <ScreenHeader navigation="back" title={translate("student.family.title")} />;

  if (isPending) {
    return (
      <ScrollScreen header={header}>
        <SkeletonList />
      </ScrollScreen>
    );
  }
  if (isError || family === undefined) {
    return (
      <ScrollScreen header={header}>
        <Banner message={translate("student.loadError")}>
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

  return (
    <ScrollScreen header={header}>
      <AppText variant="body" tone="muted">
        {translate("student.family.hint")}
      </AppText>
      <View>
        {family.members.map((member, index) => (
          <FamilyMemberRow
            key={`${member.fullName}-${member.email ?? index}`}
            member={member}
            hasDivider={index < family.members.length - 1}
          />
        ))}
      </View>
    </ScrollScreen>
  );
}
