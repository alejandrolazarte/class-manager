import { useRouter } from "expo-router";
import { ClassPackListItem } from "@/features/classPacks/components/ClassPackListItem";
import { useClassPacks } from "@/features/classPacks/useClassPacks";
import { SettingsListScreenLayout } from "@/features/settings/components/SettingsListScreenLayout";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";

export function ClassPackListScreen() {
  const router = useRouter();
  const { data: classPacks = [], isPending, isError, isRefetching, refetch } = useClassPacks(true);
  return (
    <SettingsListScreenLayout
      items={classPacks}
      isPending={isPending}
      isError={isError}
      isRefetching={isRefetching}
      onRefetch={() => refetch()}
      keyExtractor={(classPack) => classPack.id}
      renderItem={(classPack) => (
        <ClassPackListItem
          classPack={classPack}
          onPress={() => router.push(routes.classPack(classPack.id))}
        />
      )}
      emptyMessage={translate("classPacks.list.empty")}
      newItemLabel={translate("classPacks.list.newPack")}
      onNewItem={() => router.push(routes.newClassPack)}
    />
  );
}
