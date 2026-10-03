import { useRouter } from "expo-router";
import { useState } from "react";
import { FlatList, View } from "react-native";
import { ClassGroupCard } from "@/features/classGroups/components/ClassGroupCard";
import { classesOfDay } from "@/features/classGroups/classesOfDay";
import { Weekday } from "@/features/classGroups/types";
import { useActiveClassGroups } from "@/features/classGroups/useClassGroups";
import {
  weekdayLongLabel,
  weekdayOf,
  weekdayShortLabel,
  weekOrder,
} from "@/features/classGroups/weekdays";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { translate, translateCount } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { DayStrip } from "@/ui/DayStrip";
import { EmptyState } from "@/ui/EmptyState";
import { FloatingActionButton } from "@/ui/FloatingActionButton";
import { Screen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { Spinner } from "@/ui/Spinner";

export function WeeklyClassesScreen() {
  const router = useRouter();
  const canManageClassGroups = useCan(permissions.classGroupsManage);
  const [todayWeekday] = useState<Weekday>(() => weekdayOf(new Date()));
  const [selectedWeekday, setSelectedWeekday] = useState<Weekday>(todayWeekday);
  const {
    data: classGroups = [],
    isPending,
    isError,
    isRefetching,
    refetch,
  } = useActiveClassGroups();
  const dayClassGroups = classesOfDay(classGroups, selectedWeekday);
  const openNewClassGroup = () => router.push(routes.newClassGroup(selectedWeekday));

  const emptyState =
    classGroups.length === 0 ? (
      <EmptyState
        icon="classes"
        message={translate("classGroups.week.emptyTitle")}
        createActionLabel={
          canManageClassGroups ? translate("classGroups.week.newClassGroup") : undefined
        }
      />
    ) : (
      <EmptyState
        icon="noClasses"
        message={translate("classGroups.week.noClassesOnDay", {
          day: weekdayLongLabel(selectedWeekday).toLowerCase(),
        })}
      />
    );

  const header = (
    <View className="gap-[18px] pb-3">
      <ScreenHeader
        eyebrow={
          isPending ? undefined : translateCount("classGroups.week.perWeek", classGroups.length)
        }
        title={translate("classGroups.week.title")}
      />
      {isPending ? null : (
        <View className="px-5">
          <DayStrip
            days={weekOrder.map((weekday) => ({
              key: weekday,
              label: weekdayShortLabel(weekday),
              accessibilityLabel: weekdayLongLabel(weekday),
              hasClasses: classesOfDay(classGroups, weekday).length > 0,
              isToday: weekday === todayWeekday,
            }))}
            selectedKeys={[selectedWeekday]}
            onSelect={(weekday) => setSelectedWeekday(weekday as Weekday)}
          />
        </View>
      )}
      {isError ? (
        <View className="px-5">
          <Banner message={translate("common.unexpectedError")}>
            <Button
              variant="secondary"
              size="medium"
              label={translate("common.retry")}
              onPress={() => refetch()}
            />
          </Banner>
        </View>
      ) : null}
    </View>
  );

  return (
    <Screen
      overlay={
        canManageClassGroups ? (
          <FloatingActionButton
            label={translate("classGroups.week.newClassGroup")}
            onPress={openNewClassGroup}
          />
        ) : undefined
      }
    >
      {isPending ? (
        <View>
          {header}
          <Spinner className="mt-6" />
        </View>
      ) : (
        <FlatList
          data={dayClassGroups}
          keyExtractor={(classGroup) => classGroup.id}
          ListHeaderComponent={header}
          renderItem={({ item: classGroup }) => (
            <View className="px-5 pb-3">
              <ClassGroupCard
                classGroup={classGroup}
                onPress={() => router.push(routes.classGroup(classGroup.id))}
              />
            </View>
          )}
          ListEmptyComponent={isError ? null : emptyState}
          refreshing={isRefetching}
          onRefresh={() => refetch()}
          contentContainerClassName="w-full max-w-2xl self-center pb-28"
        />
      )}
    </Screen>
  );
}
