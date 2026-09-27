import { useRouter } from "expo-router";
import { useState } from "react";
import { FlatList, Pressable, View } from "react-native";
import { DaySessionCard } from "@/features/sessions/components/DaySessionCard";
import { addDays, formatLongDate, todayIsoDate } from "@/features/sessions/dates";
import { useDaySessions } from "@/features/sessions/useDaySessions";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { StepArrow } from "@/ui/StepArrow";
import { AppText } from "@/ui/AppText";
import { Spinner } from "@/ui/Spinner";

interface DayScreenProps {
  initialDate?: string;
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
    <View className="flex-1 bg-background">
      <View className="flex-row items-center justify-between bg-surface py-2">
        <StepArrow
          direction="previous"
          label={translate("sessions.day.previous")}
          onPress={() => setSessionDate(addDays(sessionDate, -1))}
        />
        <View className="items-center">
          <AppText variant="heading">{formatLongDate(sessionDate)}</AppText>
          {sessionDate === today ? null : (
            <Pressable accessibilityRole="button" onPress={() => setSessionDate(today)}>
              <AppText variant="label" tone="primary">
                {translate("sessions.day.backToToday")}
              </AppText>
            </Pressable>
          )}
        </View>
        <StepArrow
          direction="next"
          label={translate("sessions.day.next")}
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
        <Spinner className="mt-6" />
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
              <AppText variant="body" tone="muted" className="p-6 text-center">
                {translate("sessions.day.empty")}
              </AppText>
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
