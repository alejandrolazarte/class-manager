import { View } from "react-native";
import { StreakGoals } from "@/features/studentApp/components/StreakGoals";
import { StreakWeeks } from "@/features/studentApp/components/StreakWeeks";
import { useSelectedStudent } from "@/features/studentApp/AccountStudentProvider";
import { firstNameOf } from "@/features/studentApp/studentAppSchedule";
import { useStudentAppHome } from "@/features/studentApp/useStudentAppHome";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Breathing } from "@/ui/motion/Breathing";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { SkeletonDetail } from "@/ui/motion/SkeletonDetail";
import { StreakFlame } from "@/ui/StreakFlame";
import { TonalBlock } from "@/ui/TonalBlock";

export function StudentAppStreakScreen() {
  const { data: home, isPending, isError, refetch } = useStudentAppHome();
  const { student } = useSelectedStudent(home?.students ?? []);
  const title = translate("student.streak.title");

  if (isPending) {
    return (
      <ScrollScreen header={<ScreenHeader navigation="back" title={title} />}>
        <SkeletonDetail />
      </ScrollScreen>
    );
  }
  if (isError || home === undefined || student === null) {
    return (
      <ScrollScreen header={<ScreenHeader navigation="back" title={title} />}>
        <Banner message={translate("student.loadError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => refetch()}
          />
        </Banner>
      </ScrollScreen>
    );
  }

  const { attendance } = student;
  const isLit = attendance.streakWeeks > 0;
  return (
    <ScrollScreen
      header={
        <ScreenHeader navigation="back" eyebrow={firstNameOf(student.fullName)} title={title} />
      }
    >
      <TonalBlock tone="inverse">
        <View className="items-center gap-1.5 py-3">
          {isLit ? (
            <Breathing>
              <StreakFlame size="large" />
            </Breathing>
          ) : (
            <StreakFlame size="large" isLit={false} />
          )}
          <AppText variant="display" tone="inverse" className="text-center">
            {isLit
              ? translateCount("student.progress.streakWeeks", attendance.streakWeeks)
              : translate("student.streak.start")}
          </AppText>
          {attendance.bestStreakWeeks > 0 ? (
            <AppText variant="caption" tone="inverse" className="opacity-70">
              {translateCount("student.progress.bestStreak", attendance.bestStreakWeeks)}
            </AppText>
          ) : null}
        </View>
      </TonalBlock>
      {attendance.recentWeeks.length > 0 ? (
        <View className="gap-2.5">
          <SectionTitle title={translate("student.progress.recentWeeks")} />
          <StreakWeeks weeks={attendance.recentWeeks} />
        </View>
      ) : null}
      <View className="gap-2.5">
        <SectionTitle title={translate("student.streak.goals")} />
        <StreakGoals streakWeeks={attendance.streakWeeks} />
      </View>
    </ScrollScreen>
  );
}
