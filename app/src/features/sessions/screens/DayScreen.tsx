import { useRouter } from "expo-router";
import { useState } from "react";
import { FlatList, Pressable, View } from "react-native";
import { classesOfDay } from "@/features/classGroups/classesOfDay";
import { useActiveClassGroups } from "@/features/classGroups/useClassGroups";
import { weekdayOf } from "@/features/classGroups/weekdays";
import { useCurrentBrand } from "@/features/brand/BrandProvider";
import { CurrentBrandLogo } from "@/features/brand/components/CurrentBrandLogo";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { firstNameOf } from "@/features/family/familySchedule";
import { HomeTile } from "@/features/home/components/HomeTile";
import { NextSessionHero } from "@/features/home/components/NextSessionHero";
import { currentTimeLabel, nextSessionOf } from "@/features/home/nextSession";
import { useCan, useCurrentMember } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { DaySessionCard } from "@/features/sessions/components/DaySessionCard";
import { MonthCalendarGrid } from "@/features/sessions/components/MonthCalendarGrid";
import { WeekStrip } from "@/features/sessions/components/WeekStrip";
import {
  addDays,
  formatLongDate,
  monthOfDate,
  parseIsoDate,
  todayIsoDate,
} from "@/features/sessions/dates";
import { DaySession } from "@/features/sessions/types";
import { useDaySessions } from "@/features/sessions/useDaySessions";
import { useMonthCalendar } from "@/features/sessions/useMonthCalendar";
import { TeamNotificationBell } from "@/features/teamNotifications/components/TeamNotificationBell";
import { useTeamNotifications } from "@/features/teamNotifications/useTeamNotifications";
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
  const canScheduleLessons = useCan(
    permissions.privateLessonsManageAll,
    permissions.privateLessonsManageOwn,
  );
  const business = useCurrentBusiness();
  const member = useCurrentMember();
  const brandName = useCurrentBrand()?.brand?.displayName ?? business.name;
  const today = todayIsoDate();
  const [sessionDate, setSessionDate] = useState(initialDate ?? today);
  const {
    data: sessions = [],
    isPending,
    isError,
    isRefetching,
    refetch,
  } = useDaySessions(sessionDate);
  const { data: todaySessions = [] } = useDaySessions(today);
  const { data: notifications } = useTeamNotifications();
  const timeNow = currentTimeLabel();
  const nextSession = nextSessionOf(todaySessions, timeNow);
  const activeTodaySessions = todaySessions.filter((session) => !session.isCancelled);
  const unreadNotices = notifications?.unreadCount ?? 0;
  const { data: classGroups = [] } = useActiveClassGroups();
  const hasClassesOn = (isoDate: string) =>
    classesOfDay(classGroups, weekdayOf(parseIsoDate(isoDate))).length > 0;
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
          : translate("home.agenda");
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
        leading={<CurrentBrandLogo />}
        eyebrow={brandName}
        title={
          member.fullName
            ? translate("home.greeting", { name: firstNameOf(member.fullName) })
            : translate("home.title")
        }
        accessory={<TeamNotificationBell />}
      />
      <View className="gap-3 px-5">
        <NextSessionHero
          session={nextSession}
          isNow={nextSession !== null && nextSession.startTime <= timeNow}
          onOpen={() => (nextSession === null ? undefined : router.push(sessionRoute(nextSession)))}
        />
        <View className="flex-row gap-3">
          <HomeTile
            icon="today"
            iconTone="primary"
            title={translate("home.today.title")}
            value={translateCount("sessions.day.classCount", activeTodaySessions.length)}
            caption={translateCount(
              "sessions.day.studentCount",
              activeTodaySessions.reduce((total, session) => total + session.enrolledCount, 0),
            )}
          />
          <HomeTile
            icon="notifications"
            iconTone="warning"
            title={translate("home.notices.title")}
            value={
              unreadNotices > 0
                ? translateCount("home.notices.unread", unreadNotices)
                : translate("home.notices.none")
            }
            caption={translate("home.notices.caption")}
            onPress={() => router.push(routes.teamNotifications)}
          />
        </View>
      </View>
      <View className="flex-row items-center justify-between gap-3 px-5 pt-1">
        <View className="min-w-0 flex-1 gap-0.5">
          <AppText variant="eyebrow" tone="accent">
            {eyebrow}
          </AppText>
          <AppText variant="title" accessibilityRole="header">
            {formatLongDate(sessionDate)}
          </AppText>
        </View>
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
      </View>
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
          <WeekStrip
            selectedDate={sessionDate}
            today={today}
            hasClassesOn={hasClassesOn}
            onSelectDate={setSessionDate}
          />
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
        canScheduleLessons ? (
          <FloatingActionButton
            label={translate("privateLessons.new")}
            onPress={() => router.push(routes.newPrivateLesson(sessionDate))}
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
