import { View } from "react-native";
import { LevelHero } from "@/features/studentApp/components/LevelHero";
import { LevelPath } from "@/features/studentApp/components/LevelPath";
import { MedalGrid } from "@/features/studentApp/components/MedalGrid";
import { StudentChips } from "@/features/studentApp/components/StudentChips";
import { WeeksCard } from "@/features/studentApp/components/WeeksCard";
import { useSelectedStudent } from "@/features/studentApp/AccountStudentProvider";
import { allMedals, levelProgress } from "@/features/studentApp/studentAppAchievements";
import { useStudentAppHome } from "@/features/studentApp/useStudentAppHome";
import { useRefetchOnFocus } from "@/hooks/useRefetchOnFocus";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { Spinner } from "@/ui/Spinner";

function Stat({ value, label }: { value: number | string; label: string }) {
  return (
    <View className="flex-1 items-center gap-0.5">
      <AppText variant="headline">{value}</AppText>
      <AppText variant="caption" tone="muted" className="text-center">
        {label}
      </AppText>
    </View>
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
        <Spinner className="mt-6" />
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
      <Card className="flex-row px-2 py-3.5">
        <Stat
          value={attendance.attendedClasses}
          label={translate("student.progress.stats.classes")}
        />
        <Stat value={attendance.streakWeeks} label={translate("student.progress.stats.streak")} />
        <Stat
          value={`${attendance.medals.length}/${allMedals.length}`}
          label={translate("student.progress.stats.medals")}
        />
      </Card>
      <SectionTitle title={translate("student.progress.path")} />
      <LevelPath levels={home.levels} currentLevel={attendance.level} />
      <SectionTitle title={translate("student.progress.medals")} />
      <MedalGrid earned={attendance.medals} />
      <SectionTitle title={translate("student.progress.recentWeeks")} />
      <WeeksCard attendance={attendance} />
    </ScrollScreen>
  );
}
