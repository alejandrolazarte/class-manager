import { View } from "react-native";
import { LevelHero } from "@/features/studentApp/components/LevelHero";
import { LevelPath } from "@/features/studentApp/components/LevelPath";
import { MedalGrid } from "@/features/studentApp/components/MedalGrid";
import { StudentChips } from "@/features/studentApp/components/StudentChips";
import { RecentWeeks } from "@/features/studentApp/components/RecentWeeks";
import { useSelectedStudent } from "@/features/studentApp/AccountStudentProvider";
import { allMedals, levelProgress } from "@/features/studentApp/studentAppAchievements";
import { useStudentAppHome } from "@/features/studentApp/useStudentAppHome";
import { useRefetchOnFocus } from "@/hooks/useRefetchOnFocus";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { HairlineColumns } from "@/ui/HairlineColumns";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { SkeletonDetail } from "@/ui/motion/SkeletonDetail";

function Stat({ value, label }: { value: number | string; label: string }) {
  return (
    <>
      <AppText variant="headline">{value}</AppText>
      <AppText variant="footnote" tone="muted">
        {label}
      </AppText>
    </>
  );
}

export function StudentAppProgressScreen() {
  const { data: home, isPending, isError, refetch } = useStudentAppHome();
  useRefetchOnFocus(refetch);
  const { student, selectStudent } = useSelectedStudent(home?.students ?? []);
  const header = <ScreenHeader title={translate("student.progress.title")} />;

  if (isPending) {
    return (
      <ScrollScreen header={header}>
        <SkeletonDetail />
      </ScrollScreen>
    );
  }
  if (isError || home === undefined) {
    return (
      <ScrollScreen header={header}>
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
  if (student === null) {
    return (
      <ScrollScreen header={header}>
        <AppText variant="body" tone="muted">
          {translate("student.students.empty")}
        </AppText>
      </ScrollScreen>
    );
  }

  const { attendance } = student;
  return (
    <ScrollScreen header={header}>
      <StudentChips
        students={home.students}
        selectedStudentId={student.id}
        onSelect={selectStudent}
      />
      <LevelHero
        level={attendance.level}
        progress={levelProgress(home.levels, attendance.level, attendance.attendedClasses)}
      />
      <HairlineColumns>
        <Stat
          value={attendance.attendedClasses}
          label={translateCount("student.progress.stats.classes", attendance.attendedClasses)}
        />
        <Stat
          value={attendance.streakWeeks}
          label={translateCount("student.progress.stats.streak", attendance.streakWeeks)}
        />
        <Stat
          value={`${attendance.medals.length}/${allMedals.length}`}
          label={translate("student.progress.stats.medals")}
        />
      </HairlineColumns>
      <View className="gap-1">
        <SectionTitle title={translate("student.progress.path")} />
        <LevelPath levels={home.levels} currentLevel={attendance.level} />
      </View>
      <View className="gap-2.5">
        <SectionTitle title={translate("student.progress.medals")} />
        <MedalGrid earned={attendance.medals} />
      </View>
      <View className="gap-2.5">
        <SectionTitle title={translate("student.progress.recentWeeks")} />
        <RecentWeeks attendance={attendance} />
      </View>
    </ScrollScreen>
  );
}
