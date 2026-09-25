import { useRouter } from "expo-router";
import { useState } from "react";
import { ActivityIndicator, FlatList, Text, View } from "react-native";
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
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { FloatingActionButton } from "@/ui/FloatingActionButton";

export function WeeklyClassesScreen() {
  const router = useRouter();
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
      <View className="items-center gap-4 p-6">
        <Text className="text-lg text-gray-700">{translate("classGroups.week.emptyTitle")}</Text>
        <Button
          label={translate("classGroups.week.emptyCallToAction")}
          onPress={openNewClassGroup}
        />
      </View>
    ) : (
      <Text className="p-6 text-center text-base text-gray-600">
        {translate("classGroups.week.noClassesOnDay", {
          day: weekdayLongLabel(selectedWeekday).toLowerCase(),
        })}
      </Text>
    );

  return (
    <View className="flex-1 bg-gray-50">
      <View className="flex-row flex-wrap gap-2 p-4">
        {weekOrder.map((weekday) => (
          <Chip
            key={weekday}
            label={weekdayShortLabel(weekday)}
            accessibilityLabel={weekdayLongLabel(weekday)}
            isSelected={weekday === selectedWeekday}
            onPress={() => setSelectedWeekday(weekday)}
          />
        ))}
      </View>
      {isError ? (
        <View className="px-4">
          <Banner message={translate("common.unexpectedError")}>
            <Button
              variant="secondary"
              label={translate("common.retry")}
              onPress={() => refetch()}
            />
          </Banner>
        </View>
      ) : null}
      {isPending ? (
        <ActivityIndicator className="mt-6" />
      ) : (
        <FlatList
          data={dayClassGroups}
          keyExtractor={(classGroup) => classGroup.id}
          renderItem={({ item: classGroup }) => (
            <ClassGroupCard
              classGroup={classGroup}
              onPress={() => router.push(routes.classGroup(classGroup.id))}
            />
          )}
          ListEmptyComponent={isError ? null : emptyState}
          refreshing={isRefetching}
          onRefresh={() => refetch()}
          contentContainerClassName="pb-24"
        />
      )}
      <FloatingActionButton
        accessibilityLabel={translate("classGroups.week.newClassGroup")}
        onPress={openNewClassGroup}
      />
    </View>
  );
}
