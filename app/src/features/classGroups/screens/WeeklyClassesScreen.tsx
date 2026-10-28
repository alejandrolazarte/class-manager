import { useRouter } from "expo-router";
import { useState } from "react";
import { FlatList, View } from "react-native";
import { ClassGroupRow } from "@/features/classGroups/components/ClassGroupRow";
import { classesOfDay } from "@/features/classGroups/classesOfDay";
import {
  Weekday,
  weekdayLongLabel,
  weekdayOf,
  weekdayShortLabel,
  weekOrder,
} from "@/dates/weekdays";
import { useActiveClassGroups } from "@/features/classGroups/useClassGroups";
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
import { SkeletonList } from "@/ui/motion/SkeletonList";
import { AnimatedListItem } from "@/ui/motion/AnimatedListItem";

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
    <View className="gap-3.5 pb-1.5">
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
              marker: classesOfDay(classGroups, weekday).length > 0 ? "classes" : "none",
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
          <SkeletonList className="px-5" />
        </View>
      ) : (
        <FlatList
          data={dayClassGroups}
          keyExtractor={(classGroup) => classGroup.id}
          ListHeaderComponent={header}
          renderItem={({ item: classGroup, index }) => (
            <AnimatedListItem index={index} className="px-5">
              <ClassGroupRow
                classGroup={classGroup}
                onPress={() => router.push(routes.classGroup(classGroup.id))}
              />
            </AnimatedListItem>
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
