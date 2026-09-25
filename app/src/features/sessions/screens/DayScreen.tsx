import { useRouter } from "expo-router";
import { useState } from "react";
import { ActivityIndicator, FlatList, Pressable, Text, View } from "react-native";
import { DaySessionCard } from "@/features/sessions/components/DaySessionCard";
import { addDays, formatLongDate, todayIsoDate } from "@/features/sessions/dates";
import { useDaySessions } from "@/features/sessions/useDaySessions";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";

interface DayScreenProps {
  initialDate?: string;
}

function DayArrow({
  label,
  glyph,
  onPress,
}: {
  label: string;
  glyph: string;
  onPress: () => void;
}) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={label}
      onPress={onPress}
      className="px-4 py-2"
    >
      <Text className="text-2xl text-brand">{glyph}</Text>
    </Pressable>
  );
}

export function DayScreen({ initialDate }: DayScreenProps) {
  const router = useRouter();
  const today = todayIsoDate();
  const [sessionDate, setSessionDate] = useState(initialDate ?? today);
  const {
    data: sessions = [],
    isPending,
    isError,
    isRefetching,
    refetch,
  } = useDaySessions(sessionDate);

  return (
    <View className="flex-1 bg-gray-50">
      <View className="flex-row items-center justify-between bg-white py-2">
        <DayArrow
          label={translate("sessions.day.previous")}
          glyph="‹"
          onPress={() => setSessionDate(addDays(sessionDate, -1))}
        />
        <View className="items-center">
          <Text className="text-lg font-semibold text-gray-900">{formatLongDate(sessionDate)}</Text>
          {sessionDate === today ? null : (
            <Pressable accessibilityRole="button" onPress={() => setSessionDate(today)}>
              <Text className="text-sm font-medium text-brand">
                {translate("sessions.day.backToToday")}
              </Text>
            </Pressable>
          )}
        </View>
        <DayArrow
          label={translate("sessions.day.next")}
          glyph="›"
          onPress={() => setSessionDate(addDays(sessionDate, 1))}
        />
      </View>
      {isError ? (
        <View className="p-4">
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
          data={sessions}
          keyExtractor={(session) => session.classGroupId}
          renderItem={({ item: session }) => (
            <DaySessionCard
              session={session}
              onPress={() => router.push(routes.session(session.classGroupId, session.date))}
            />
          )}
          ListEmptyComponent={
            isError ? null : (
              <Text className="p-6 text-center text-base text-gray-600">
                {translate("sessions.day.empty")}
              </Text>
            )
          }
          refreshing={isRefetching}
          onRefresh={() => refetch()}
          contentContainerClassName="pb-12"
        />
      )}
    </View>
  );
}
