import { useRouter } from "expo-router";
import { useState } from "react";
import { FlatList, Pressable, View } from "react-native";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { DaySessionCard } from "@/features/sessions/components/DaySessionCard";
import { MonthCalendarGrid } from "@/features/sessions/components/MonthCalendarGrid";
import { WeekStrip } from "@/features/sessions/components/WeekStrip";
import { addDays, formatLongDate, monthOfDate, todayIsoDate } from "@/features/sessions/dates";
import { DaySession } from "@/features/sessions/types";
import { useDaySessions } from "@/features/sessions/useDaySessions";
import { useMonthCalendar } from "@/features/sessions/useMonthCalendar";
import { addMonths, formatMonth } from "@/features/fees/months";
import { translate, translateCount } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { EmptyState } from "@/ui/EmptyState";
import { FloatingActionButton } from "@/ui/FloatingActionButton";
import { IconButton } from "@/ui/IconButton";
import { Screen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { Spinner } from "@/ui/Spinner";
import { StepArrow } from "@/ui/StepArrow";

interface DayScreenProps {
  initialDate?: string;
}

const summarySeparator = " · ";

function sessionRoute(session: DaySession): string {
  return session.kind === "Private"
    ? routes.privateLesson(session.privateLessonId ?? "")
    : routes.session(session.classGroupId ?? "", session.date);
}

export function DayScreen({ initialDate }: DayScreenProps) {
  const router = useRouter();
  const business = useCurrentBusiness();
  const today = todayIsoDate();
  const [sessionDate, setSessionDate] = useState(initialDate ?? today);
  const {
    data: sessions = [],
    isPending,
    isError,
    isRefetching,
    refetch,
  } = useDaySessions(sessionDate);
  const [isMonthView, setIsMonthView] = useState(false);
  const [calendarMonth, setCalendarMonth] = useState(() => monthOfDate(sessionDate));
  const { data: monthCalendar } = useMonthCalendar(calendarMonth, isMonthView);

  const toggleMonthView = () => {
    setCalendarMonth(monthOfDate(sessionDate));
    setIsMonthView(!isMonthView);
  };
  const selectCalendarDate = (isoDate: string) => {
    setSessionDate(isoDate);
    setIsMonthView(false);
  };

  const eyebrow =
    sessionDate === today
      ? translate("sessions.day.title")
      : sessionDate === addDays(today, 1)
        ? translate("sessions.day.tomorrow")
        : sessionDate === addDays(today, -1)
          ? translate("sessions.day.yesterday")
          : business.name;
  const expectedStudentCount = sessions
    .filter((session) => !session.isCancelled)
    .reduce((total, session) => total + session.enrolledCount, 0);
  const summary =
    sessions.length === 0
      ? translate("sessions.day.noClasses")
      : [
          translateCount("sessions.day.classCount", sessions.length),
          translateCount("sessions.day.studentCount", expectedStudentCount),
        ].join(summarySeparator);

  const header = (
    <View className="gap-[18px] pb-3">
      <ScreenHeader
        eyebrow={eyebrow}
        title={formatLongDate(sessionDate)}
        accessory={
          <View className="flex-row gap-1">
            <IconButton
              variant="outlined"
              icon={isMonthView ? "weekView" : "monthView"}
              tone="primary"
              accessibilityLabel={translate(
                isMonthView ? "sessions.calendar.hide" : "sessions.calendar.show",
              )}
              onPress={toggleMonthView}
            />
            {isMonthView ? (
              <>
                <StepArrow
                  direction="previous"
                  label={translate("sessions.calendar.previousMonth")}
                  onPress={() => setCalendarMonth(addMonths(calendarMonth, -1))}
                />
                <StepArrow
                  direction="next"
                  label={translate("sessions.calendar.nextMonth")}
                  onPress={() => setCalendarMonth(addMonths(calendarMonth, 1))}
                />
              </>
            ) : (
              <>
                <StepArrow
                  direction="previous"
                  label={translate("sessions.day.previous")}
                  onPress={() => setSessionDate(addDays(sessionDate, -1))}
                />
                <StepArrow
                  direction="next"
                  label={translate("sessions.day.next")}
                  onPress={() => setSessionDate(addDays(sessionDate, 1))}
                />
              </>
            )}
          </View>
        }
      />
      <View className="gap-[18px] px-5">
        {isMonthView ? (
          <View className="gap-2">
            <AppText variant="heading" className="text-center">
              {formatMonth(calendarMonth)}
            </AppText>
            <MonthCalendarGrid
              month={calendarMonth}
              days={monthCalendar?.days ?? []}
              selectedDate={sessionDate}
              today={today}
              onSelectDate={selectCalendarDate}
            />
          </View>
        ) : (
          <WeekStrip selectedDate={sessionDate} today={today} onSelectDate={setSessionDate} />
        )}
        <View className="min-h-6 flex-row items-center justify-between">
          <AppText variant="bodyStrong" tone="muted" className="font-label">
            {isPending ? "" : summary}
          </AppText>
          {sessionDate === today ? null : (
            <Pressable accessibilityRole="button" onPress={() => setSessionDate(today)}>
              <AppText variant="link" tone="primary">
                {translate("sessions.day.backToToday")}
              </AppText>
            </Pressable>
          )}
        </View>
        {isError ? (
          <Banner message={translate("common.unexpectedError")}>
            <Button
              variant="secondary"
              size="medium"
              label={translate("common.retry")}
              onPress={() => refetch()}
            />
          </Banner>
        ) : null}
      </View>
    </View>
  );

  return (
    <Screen
      overlay={
        <FloatingActionButton
          label={translate("privateLessons.new")}
          onPress={() => router.push(routes.newPrivateLesson(sessionDate))}
        />
      }
    >
      {isPending ? (
        <View>
          {header}
          <Spinner className="mt-6" />
        </View>
      ) : (
        <FlatList
          data={sessions}
          keyExtractor={(session) => session.privateLessonId ?? session.classGroupId ?? ""}
          ListHeaderComponent={header}
          renderItem={({ item: session }) => (
            <View className="px-5 pb-3">
              <DaySessionCard
                session={session}
                onPress={() => router.push(sessionRoute(session))}
              />
            </View>
          )}
          ListEmptyComponent={
            isError ? null : <EmptyState icon="brand" message={translate("sessions.day.empty")} />
          }
          refreshing={isRefetching}
          onRefresh={() => refetch()}
          contentContainerClassName="w-full max-w-2xl self-center pb-28"
        />
      )}
    </Screen>
  );
}
