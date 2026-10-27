import { LevelPath } from "@/features/studentApp/components/LevelPath";
import { useSelectedStudent } from "@/features/studentApp/AccountStudentProvider";
import { firstNameOf } from "@/features/studentApp/studentAppSchedule";
import { useStudentAppHome } from "@/features/studentApp/useStudentAppHome";
import { translate, translateCount } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SkeletonDetail } from "@/ui/motion/SkeletonDetail";
import { TonalBlock } from "@/ui/TonalBlock";

export function StudentAppLevelsScreen() {
  const { data: home, isPending, isError, refetch } = useStudentAppHome();
  const { student } = useSelectedStudent(home?.students ?? []);
  const title = translate("student.progress.path");

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
  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="back"
          eyebrow={firstNameOf(student.fullName)}
          title={title}
          subtitle={translateCount("student.levels.summary", attendance.attendedClasses, {
            level: attendance.level,
            total: home.levels.length,
          })}
        />
      }
    >
      <TonalBlock tone="surface">
        <LevelPath
          levels={home.levels}
          currentLevel={attendance.level}
          attendedClasses={attendance.attendedClasses}
        />
      </TonalBlock>
    </ScrollScreen>
  );
}
