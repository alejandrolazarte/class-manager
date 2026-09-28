import { useRouter } from "expo-router";
import { useState } from "react";
import { FlatList, Pressable, ScrollView, View } from "react-native";
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
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { EmptyState } from "@/ui/EmptyState";
import { FloatingActionButton } from "@/ui/FloatingActionButton";
import { Screen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { Spinner } from "@/ui/Spinner";

interface DayFilterChipProps {
  weekday: Weekday;
  classCount: number;
  isSelected: boolean;
  onPress: () => void;
}

function DayFilterChip({ weekday, classCount, isSelected, onPress }: DayFilterChipProps) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={weekdayLongLabel(weekday)}
      accessibilityState={{ selected: isSelected }}
      onPress={onPress}
      className={`h-10 min-w-12 flex-row items-center justify-center gap-1.5 rounded-full px-3.5 ${isSelected ? "bg-primary" : "border-[1.5px] border-border bg-surface"}`}
    >
      <AppText variant="link" tone={isSelected ? "onPrimary" : "default"}>
        {weekdayShortLabel(weekday)}
      </AppText>
      {isSelected || classCount === 0 ? null : (
        <AppText variant="footnote" tone="subtle" className="font-strong text-[11px]">
          {classCount}
        </AppText>
      )}
    </Pressable>
  );
}

export function WeeklyClassesScreen() {
  const router = useRouter();
  const canManageClassGroups = useCan(permissions.classGroupsManage);
  const [selectedWeekday, setSelectedWeekday] = useState<Weekday>(() => weekdayOf(new Date()));
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
      <EmptyState icon="classes" message={translate("classGroups.week.emptyTitle")}>
        {canManageClassGroups ? (
          <Button
            label={translate("classGroups.week.emptyCallToAction")}
            onPress={openNewClassGroup}
          />
        ) : null}
      </EmptyState>
    ) : (
      <EmptyState
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
        <ScrollView
          horizontal
          showsHorizontalScrollIndicator={false}
          contentContainerClassName="gap-1.5 px-5"
        >
          {weekOrder.map((weekday) => (
            <DayFilterChip
              key={weekday}
              weekday={weekday}
              classCount={classesOfDay(classGroups, weekday).length}
              isSelected={weekday === selectedWeekday}
              onPress={() => setSelectedWeekday(weekday)}
            />
          ))}
        </ScrollView>
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
