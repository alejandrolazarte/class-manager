import { useRouter } from "expo-router";
import { useState } from "react";
import { Pressable, RefreshControl, ScrollView, View } from "react-native";
import { useCurrentBrand } from "@/features/brand/BrandProvider";
import { CurrentBrandLogo } from "@/features/brand/components/CurrentBrandLogo";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { classesOfDay } from "@/features/classGroups/classesOfDay";
import { useActiveClassGroups } from "@/features/classGroups/useClassGroups";
import { weekdayOf, weekdayShortLabel } from "@/features/classGroups/weekdays";
import { firstNameOf } from "@/features/family/familySchedule";
import { addMonths, formatMonth } from "@/features/fees/months";
import {
  currentTimeLabel,
  relativeDayLabel,
  sessionTimingOf,
  todayHighlightOf,
} from "@/features/home/agenda";
import { TodayBanner } from "@/features/home/components/TodayBanner";
import { useCan, useCurrentMember } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { AgendaCalendar, AgendaView } from "@/features/sessions/components/AgendaCalendar";
import { AgendaSessionCard } from "@/features/sessions/components/AgendaSessionCard";
import {
  addDays,
  dayOfMonth,
  monthOfDate,
  parseIsoDate,
  todayIsoDate,
} from "@/features/sessions/dates";
import { DaySession } from "@/features/sessions/types";
import { useDaySessions } from "@/features/sessions/useDaySessions";
import { useMonthCalendar } from "@/features/sessions/useMonthCalendar";
import { TeamNotificationBell } from "@/features/teamNotifications/components/TeamNotificationBell";
import { translate, translateCount, TranslationKey } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { FloatingActionButton } from "@/ui/FloatingActionButton";
import { Icon } from "@/ui/Icon";
import { Screen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SegmentedControl } from "@/ui/SegmentedControl";
import { Spinner } from "@/ui/Spinner";

interface DayScreenProps {
  initialDate?: string;
}

const summarySeparator = " · ";
const studentNameSeparator = ", ";
const daysPerWeek = 7;
const shortMonthLength = 3;

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

function shortDateLabel(isoDate: string): string {
  const monthName = translate(
    `months.${parseIsoDate(isoDate).getMonth() + 1}` as TranslationKey,
  ).slice(0, shortMonthLength);
  return `${weekdayShortLabel(weekdayOf(parseIsoDate(isoDate)))} ${dayOfMonth(isoDate)} ${monthName}`;
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
  const [calendarMonth, setCalendarMonth] = useState(() => monthOfDate(sessionDate));
  const {
    data: sessions = [],
    isPending,
    isError,
    isPlaceholderData,
    isRefetching,
    refetch,
  } = useDaySessions(sessionDate);
  const { data: todaySessions = [] } = useDaySessions(today);
  const { data: classGroups = [] } = useActiveClassGroups();
  const isMonthView = view === "month";
  const { data: monthCalendar } = useMonthCalendar(calendarMonth, isMonthView);
  const hasClassesOn = (isoDate: string) =>
    classesOfDay(classGroups, weekdayOf(parseIsoDate(isoDate))).length > 0;

  const openSession = (session: DaySession) => router.push(sessionRoute(session));
  const openNewPrivateLesson = () => router.push(routes.newPrivateLesson(sessionDate));
  const changeView = (nextView: AgendaView) => {
    setCalendarMonth(monthOfDate(sessionDate));
    setView(nextView);
  };
  const selectDate = (isoDate: string) => {
    setSessionDate(isoDate);
    setView("day");
  };
  const step = (direction: number) =>
    isMonthView
      ? setCalendarMonth(addMonths(calendarMonth, direction))
      : setSessionDate(addDays(sessionDate, direction * daysPerWeek));
  const goToToday = () => {
    setSessionDate(today);
    setCalendarMonth(monthOfDate(today));
    setView("day");
  };

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
  const isShowingToday = sessionDate === today && !isMonthView;

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
            <TodayBanner
              highlight={todayHighlightOf(todaySessions, today, timeNow)}
              timeNow={timeNow}
              titleOf={sessionTitle}
              onOpen={openSession}
            />
            <View className="mt-1.5 h-12 flex-row items-center justify-between gap-3">
              <View className="min-w-0 flex-1">
                <AppText variant="eyebrow" tone="accent" numberOfLines={1}>
                  {isMonthView
                    ? translate("home.view.calendar")
                    : relativeDayLabel(sessionDate, today)}
                </AppText>
                <AppText variant="headline" accessibilityRole="header" numberOfLines={1}>
                  {isMonthView ? formatMonth(calendarMonth) : shortDateLabel(sessionDate)}
                </AppText>
              </View>
              <SegmentedControl
                isCompact
                options={[
                  { value: "day", label: translate("home.view.day") },
                  { value: "month", label: translate("home.view.month") },
                ]}
                selectedValue={view}
                onChange={changeView}
              />
            </View>
            <AgendaCalendar
              view={view}
              selectedDate={sessionDate}
              today={today}
              month={calendarMonth}
              monthDays={monthCalendar?.days ?? []}
              hasClassesOn={hasClassesOn}
              onSelectDate={selectDate}
              onPrevious={() => step(-1)}
              onNext={() => step(1)}
            />
            <View className="h-7 flex-row items-center justify-between">
              <AppText variant="bodyStrong" tone="muted" className="font-strong">
                {isPending ? "" : summary}
              </AppText>
              <Pressable
                accessibilityRole="button"
                accessibilityLabel={translate("sessions.day.backToToday")}
                accessibilityElementsHidden={isShowingToday}
                importantForAccessibility={isShowingToday ? "no-hide-descendants" : "auto"}
                disabled={isShowingToday}
                onPress={goToToday}
                className={`h-7 flex-row items-center gap-1 rounded-full bg-primary-soft px-3 ${isShowingToday ? "opacity-0" : ""}`}
              >
                <Icon name="today" size="small" tone="primary-soft-foreground" />
                <AppText variant="badge" tone="primarySoft">
                  {translate("home.backToToday")}
                </AppText>
              </Pressable>
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
        <View className={`min-h-[276px] gap-3 px-5 ${isPlaceholderData ? "opacity-50" : ""}`}>
          {isPending ? <Spinner className="mt-6" /> : null}
          {!isPending && !isError && sessions.length === 0 ? (
            <Card className="h-[84px] flex-row items-center gap-3 px-4">
              <View className="h-10 w-10 items-center justify-center rounded-xl bg-muted">
                <Icon name="noClasses" tone="muted-foreground" />
              </View>
              <View className="min-w-0 flex-1">
                <AppText variant="bodyStrong">{translate("home.free.title")}</AppText>
                <AppText variant="caption" tone="subtle">
                  {translate("home.free.body")}
                </AppText>
              </View>
            </Card>
          ) : null}
          {sessions.map((session) => (
            <AgendaSessionCard
              key={session.privateLessonId ?? session.classGroupId ?? session.startTime}
              session={session}
              title={sessionTitle(session)}
              timing={sessionTimingOf(session, today, timeNow)}
              timeNow={timeNow}
              isToday={session.date === today}
              onPress={() => openSession(session)}
            />
          ))}
        </View>
      </ScrollView>
    </Screen>
  );
}
