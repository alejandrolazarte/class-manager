import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { StudentAppMeasurementsSection } from "@/features/measurements/components/StudentAppMeasurementsSection";
import { useStudentAppMeasurements } from "@/features/measurements/useMeasurements";
import { LevelHero } from "@/features/studentApp/components/LevelHero";
import { MedalCelebration } from "@/features/studentApp/components/MedalCelebration";
import { MedalGrid } from "@/features/studentApp/components/MedalGrid";
import { NextMedalBanner } from "@/features/studentApp/components/NextMedalBanner";
import { StreakCard } from "@/features/studentApp/components/StreakCard";
import { StudentChips } from "@/features/studentApp/components/StudentChips";
import { useSelectedStudent } from "@/features/studentApp/AccountStudentProvider";
import {
  allMedals,
  levelProgress,
  nextMedalGoal,
} from "@/features/studentApp/studentAppAchievements";
import { useNewMedal } from "@/features/studentApp/useNewMedal";
import { useStudentAppHome } from "@/features/studentApp/useStudentAppHome";
import { useRefetchOnFocus } from "@/hooks/useRefetchOnFocus";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { FloatingActionButton } from "@/ui/FloatingActionButton";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { SkeletonDetail } from "@/ui/motion/SkeletonDetail";
import { SkeletonList } from "@/ui/motion/SkeletonList";
import { Tabs } from "@/ui/Tabs";

type ProgressSection = "constancy" | "measurements";

export function StudentAppProgressScreen() {
  const router = useRouter();
  const [section, setSection] = useState<ProgressSection>("constancy");
  const { data: home, isPending, isError, refetch } = useStudentAppHome();
  const measurements = useStudentAppMeasurements();
  useRefetchOnFocus(refetch);
  useRefetchOnFocus(measurements.refetch);
  const { student, selectStudent } = useSelectedStudent(home?.students ?? []);
  const { newMedal, dismissNewMedal } = useNewMedal(student);
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
  const studentMeasurements =
    measurements.data?.students.find((candidate) => candidate.studentId === student.id)
      ?.measurements ?? [];
  const canRecordMeasurements = studentMeasurements.some(
    (item) => item.measurement.isRecordedByStudents,
  );
  const isShowingMeasurements = section === "measurements";
  const nextMedal = nextMedalGoal(attendance);
  return (
    <ScrollScreen
      header={header}
      hasFloatingAction={isShowingMeasurements && canRecordMeasurements}
      overlay={
        isShowingMeasurements && canRecordMeasurements ? (
          <FloatingActionButton
            label={translate("student.measurements.record")}
            onPress={() => router.push(routes.studentAppRecordMeasurement(student.id))}
          />
        ) : undefined
      }
    >
      <StudentChips
        students={home.students}
        selectedStudentId={student.id}
        onSelect={selectStudent}
      />
      <Tabs
        options={[
          { value: "constancy", label: translate("student.progress.tabs.constancy") },
          { value: "measurements", label: translate("student.progress.tabs.measurements") },
        ]}
        selectedValue={section}
        onChange={setSection}
      />
      {isShowingMeasurements ? (
        measurements.isPending ? (
          <SkeletonList />
        ) : measurements.isError ? (
          <Banner message={translate("student.loadError")}>
            <Button
              variant="secondary"
              size="medium"
              label={translate("common.retry")}
              onPress={() => measurements.refetch()}
            />
          </Banner>
        ) : (
          <StudentAppMeasurementsSection
            studentId={student.id}
            measurements={studentMeasurements}
          />
        )
      ) : (
        <>
          <LevelHero
            level={attendance.level}
            attendedClasses={attendance.attendedClasses}
            progress={levelProgress(home.levels, attendance.level, attendance.attendedClasses)}
            onSeeAllLevels={() => router.push(routes.studentAppLevels)}
          />
          <StreakCard
            attendance={attendance}
            onSeeStreak={() => router.push(routes.studentAppStreak)}
          />
          {nextMedal === null ? null : <NextMedalBanner goal={nextMedal} />}
          <View className="gap-2.5">
            <View className="flex-row items-baseline justify-between">
              <SectionTitle title={translate("student.progress.medals")} />
              <AppText variant="caption" tone="muted">
                {translate("student.progress.medalsCount", {
                  count: attendance.medals.length,
                  total: allMedals.length,
                })}
              </AppText>
            </View>
            <MedalGrid attendance={attendance} />
          </View>
          {newMedal === null ? null : (
            <MedalCelebration
              medal={newMedal}
              nextMedal={nextMedal?.medal ?? null}
              onClose={dismissNewMedal}
            />
          )}
        </>
      )}
    </ScrollScreen>
  );
}
