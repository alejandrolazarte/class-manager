import { useRouter } from "expo-router";
import { useState } from "react";
import { RefreshControl, ScrollView, View } from "react-native";
import { AccountSwitcher } from "@/features/authentication/components/AccountSwitcher";
import { useCurrentBrand } from "@/features/brand/BrandProvider";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { weekdayLongLabel, weekdayOf } from "@/dates/weekdays";
import { firstNameOf } from "@/features/studentApp/studentAppSchedule";
import {
  agendaHeadingOf,
  currentTimeLabel,
  sessionTimingOf,
  todayHighlightOf,
} from "@/features/home/agenda";
import { TodayBanner } from "@/features/home/components/TodayBanner";
import { useCan, useCurrentMember } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { AgendaSessionRow } from "@/features/sessions/components/AgendaSessionRow";
import {
  addDays,
  addMonthsToDate,
  dayOfMonth,
  monthOfDate,
  parseIsoDate,
  todayIsoDate,
  weekOf,
} from "@/dates/dates";
import { CalendarDay, DaySession } from "@/features/sessions/types";
import { useDaySessions } from "@/features/sessions/useDaySessions";
import { useMonthCalendar } from "@/features/sessions/useMonthCalendar";
import { TeamNotificationBell } from "@/features/teamNotifications/components/TeamNotificationBell";
import { translate, translateCount } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { HairlineRow } from "@/ui/HairlineRow";
import { IconCircle } from "@/ui/IconCircle";
import { FloatingActionButton } from "@/ui/FloatingActionButton";
import { Screen } from "@/ui/Screen";
import { TonalButton } from "@/ui/TonalButton";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { AgendaCalendar, AgendaMonthDay, AgendaView } from "@/ui/AgendaCalendar";
import { DayMarker } from "@/ui/DayMarker";
import { SubscriptionNotice } from "@/features/subscriptions/components/SubscriptionNotice";
import { SkeletonList } from "@/ui/motion/SkeletonList";
import { AnimatedListItem } from "@/ui/motion/AnimatedListItem";

interface DayScreenProps {
  initialDate?: string;
}

const summarySeparator = " · ";
const studentNameSeparator = ", ";
const daysPerWeek = 7;
const labelSeparator = " · ";

function sessionRoute(session: DaySession): string {
  return session.kind === "Private"
    ? routes.privateLesson(session.privateLessonId ?? "")
    : routes.session(session.classGroupId ?? "", session.date);
}

function sessionTitle(session: DaySession): string {
  return session.kind === "Private"
    ? session.studentNames.join(studentNameSeparator)
    : session.classGroupName;
}

function dayLabel(isoDate: string): string {
  return translate("sessions.day.pickDay", {
    weekday: weekdayLongLabel(weekdayOf(parseIsoDate(isoDate))),
    day: dayOfMonth(isoDate),
  });
}

function monthDayLabel(isoDate: string, calendarDay: CalendarDay | undefined): string {
  const parts = [
    translate("sessions.calendar.dayLabel", {
      weekday: weekdayLongLabel(weekdayOf(parseIsoDate(isoDate))),
      day: dayOfMonth(isoDate),
    }),
  ];
  if (calendarDay) {
    parts.push(translateCount("sessions.day.classCount", calendarDay.classCount));
    if (calendarDay.pendingAttendanceCount > 0) {
      parts.push(translate("sessions.calendar.pending"));
    }
    if (calendarDay.cancelledCount > 0) {
      parts.push(translate("sessions.calendar.cancelled"));
    }
  }
  return parts.join(labelSeparator);
}

function markerOf(calendarDay: CalendarDay | undefined): DayMarker {
  if ((calendarDay?.pendingAttendanceCount ?? 0) > 0) {
    return "pending";
  }
  return (calendarDay?.classCount ?? 0) > 0 ? "classes" : "none";
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
  const timeNow = currentTimeLabel();
  const [sessionDate, setSessionDate] = useState(initialDate ?? today);
  const [view, setView] = useState<AgendaView>("day");
  const {
    data: sessions = [],
    isPending,
    isError,
    isPlaceholderData,
    isRefetching,
    refetch,
  } = useDaySessions(sessionDate);
  const { data: todaySessions = [] } = useDaySessions(today);
  const isMonthView = view === "month";
  const calendarMonth = monthOfDate(sessionDate);
  const { data: monthCalendar } = useMonthCalendar(calendarMonth, isMonthView);
  const week = weekOf(sessionDate);
  const { data: weekStartCalendar } = useMonthCalendar(monthOfDate(week[0]!), !isMonthView);
  const { data: weekEndCalendar } = useMonthCalendar(
    monthOfDate(week[week.length - 1]!),
    !isMonthView,
  );
  const datesWithClasses = new Set(
    [...(weekStartCalendar?.days ?? []), ...(weekEndCalendar?.days ?? [])]
      .filter((calendarDay) => calendarDay.classCount > 0)
      .map((calendarDay) => calendarDay.date),
  );
  const hasClassesOn = (isoDate: string) => datesWithClasses.has(isoDate);
  const monthCalendarDays = new Map(
    (monthCalendar?.days ?? []).map((calendarDay) => [calendarDay.date, calendarDay]),
  );
  const monthDayOf = (isoDate: string): AgendaMonthDay => {
    const calendarDay = monthCalendarDays.get(isoDate);
    return {
      marker: markerOf(calendarDay),
      isCancelled: (calendarDay?.cancelledCount ?? 0) > 0,
      accessibilityLabel: monthDayLabel(isoDate, calendarDay),
    };
  };
  const heading = agendaHeadingOf(view, sessionDate, today);

  const openSession = (session: DaySession) => router.push(sessionRoute(session));
  const openNewPrivateLesson = () => router.push(routes.newPrivateLesson(sessionDate));
  const step = (direction: number) =>
    setSessionDate(
      isMonthView
        ? addMonthsToDate(sessionDate, direction)
        : addDays(sessionDate, direction * daysPerWeek),
    );
  const goToToday = () => setSessionDate(today);

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
  const isShowingToday = sessionDate === today;

  return (
    <Screen
      overlay={
        canScheduleLessons ? (
          <FloatingActionButton
            label={translate("privateLessons.new")}
            onPress={openNewPrivateLesson}
          />
        ) : undefined
      }
    >
      <ScrollView
        contentContainerClassName="w-full max-w-2xl self-center pb-28"
        refreshControl={<RefreshControl refreshing={isRefetching} onRefresh={() => refetch()} />}
      >
        <View className="gap-3.5 pb-3">
          <ScreenHeader
            leading={<AccountSwitcher displayName={business.name} />}
            eyebrow={brandName}
            title={
              member.fullName
                ? translate("home.greeting", { name: firstNameOf(member.fullName) })
                : translate("home.title")
            }
            accessory={<TeamNotificationBell />}
          />
          <View className="gap-3 px-5">
            <SubscriptionNotice />
            <TodayBanner
              highlight={todayHighlightOf(todaySessions, today, timeNow)}
              timeNow={timeNow}
              titleOf={sessionTitle}
              onOpen={openSession}
            />
            <AgendaCalendar
              view={view}
              onChangeView={setView}
              eyebrow={heading.eyebrow}
              title={heading.title}
              selectedDate={sessionDate}
              today={today}
              weekDayOf={(isoDate) => ({
                marker: hasClassesOn(isoDate) ? "classes" : "none",
                accessibilityLabel: dayLabel(isoDate),
              })}
              monthDayOf={monthDayOf}
              onSelectDate={setSessionDate}
              onPrevious={() => step(-1)}
              onNext={() => step(1)}
            />
            <View className="h-7 flex-row items-center justify-between">
              <AppText variant="caption" tone="subtle">
                {isPending ? "" : summary}
              </AppText>
              <TonalButton
                label={translate("home.backToToday")}
                accessibilityLabel={translate("sessions.day.backToToday")}
                icon="today"
                isHidden={isShowingToday}
                onPress={goToToday}
              />
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
        <View className={`min-h-[276px] px-5 ${isPlaceholderData ? "opacity-50" : ""}`}>
          {isPending ? <SkeletonList /> : null}
          {!isPending && !isError && sessions.length === 0 ? (
            <HairlineRow className="flex-row items-center gap-3 py-[18px]">
              <IconCircle icon="noClasses" />
              <View className="min-w-0 flex-1">
                <AppText variant="bodyStrong">{translate("home.free.title")}</AppText>
                <AppText variant="caption" tone="subtle">
                  {translate("home.free.body")}
                </AppText>
              </View>
            </HairlineRow>
          ) : null}
          {sessions.map((session, index) => (
            <AnimatedListItem
              key={session.privateLessonId ?? session.classGroupId ?? session.startTime}
              index={index}
            >
              <AgendaSessionRow
                session={session}
                title={sessionTitle(session)}
                timing={sessionTimingOf(session, today, timeNow)}
                timeNow={timeNow}
                isToday={session.date === today}
                isFirst={index === 0}
                isLast={index === sessions.length - 1}
                onPress={() => openSession(session)}
              />
            </AnimatedListItem>
          ))}
        </View>
      </ScrollView>
    </Screen>
  );
}
