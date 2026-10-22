import { useRouter } from "expo-router";
import { ClassPackListItem } from "@/features/classPacks/components/ClassPackListItem";
import { useClassPacks } from "@/features/classPacks/useClassPacks";
import { useCan, useRoleCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { SettingsListScreenLayout } from "@/features/settings/components/SettingsListScreenLayout";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { usePlanAllowing } from "@/features/subscriptions/PlanOfferProvider";
import { featureCodes } from "@/features/subscriptions/subscriptionCodes";
import { useFeature } from "@/features/subscriptions/useSubscription";

export function ClassPackListScreen() {
  const hasFeature = useFeature(featureCodes.classPacks);
  const router = useRouter();
  const canManageClassPacks = useCan(permissions.classPacksManage);
  const isRoleAllowedToManage = useRoleCan(permissions.classPacksManage);
  const withinClassPacksPlan = usePlanAllowing(featureCodes.classPacks, hasFeature);
  const { data: classPacks = [], isPending, isError, isRefetching, refetch } = useClassPacks(true);
  return (
    <SettingsListScreenLayout
      title={translate("classPacks.list.title")}
      items={classPacks}
      isPending={isPending}
      isError={isError}
      isRefetching={isRefetching}
      onRefetch={() => refetch()}
      keyExtractor={(classPack) => classPack.id}
      renderItem={(classPack) => (
        <ClassPackListItem
          classPack={classPack}
          onPress={
            canManageClassPacks ? () => router.push(routes.classPack(classPack.id)) : undefined
          }
        />
      )}
      emptyIcon="classPacks"
      emptyMessage={translate("classPacks.list.empty")}
      newItemLabel={translate("classPacks.list.newPack")}
      onNewItem={
        isRoleAllowedToManage
          ? withinClassPacksPlan(() => router.push(routes.newClassPack))
          : undefined
      }
    />
  );
}
