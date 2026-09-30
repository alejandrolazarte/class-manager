import { View } from "react-native";
import { LevelHero } from "@/features/family/components/LevelHero";
import { LevelPath } from "@/features/family/components/LevelPath";
import { MedalGrid } from "@/features/family/components/MedalGrid";
import { StudentChips } from "@/features/family/components/StudentChips";
import { WeeksCard } from "@/features/family/components/WeeksCard";
import { useSelectedStudent } from "@/features/family/FamilyStudentProvider";
import { allMedals, levelProgress } from "@/features/family/familyAchievements";
import { useFamilyHome } from "@/features/family/useFamilyHome";
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

export function FamilyProgressScreen() {
  const { data: home, isPending, isError, refetch } = useFamilyHome();
  useRefetchOnFocus(refetch);
  const { student, selectStudent } = useSelectedStudent(home?.students ?? []);
  const header = <ScreenHeader title={translate("family.progress.title")} />;

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
        <Banner message={translate("family.loadError")}>
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
          {translate("family.students.empty")}
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
          label={translate("family.progress.stats.classes")}
        />
        <Stat value={attendance.streakWeeks} label={translate("family.progress.stats.streak")} />
        <Stat
          value={`${attendance.medals.length}/${allMedals.length}`}
          label={translate("family.progress.stats.medals")}
        />
      </Card>
      <SectionTitle title={translate("family.progress.path")} />
      <LevelPath levels={home.levels} currentLevel={attendance.level} />
      <SectionTitle title={translate("family.progress.medals")} />
      <MedalGrid earned={attendance.medals} />
      <SectionTitle title={translate("family.progress.recentWeeks")} />
      <WeeksCard attendance={attendance} />
    </ScrollScreen>
  );
}
