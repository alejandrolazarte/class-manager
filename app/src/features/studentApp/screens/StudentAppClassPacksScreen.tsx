import { useRouter } from "expo-router";
import { View } from "react-native";
import { PackClassRow } from "@/features/studentApp/components/PackClassRow";
import { PackPurchaseCard } from "@/features/studentApp/components/PackPurchaseCard";
import { firstNameOf } from "@/features/studentApp/studentAppSchedule";
import { useStudentAppClassPacks } from "@/features/studentApp/useStudentAppClassPacks";
import { useStudentAppHome } from "@/features/studentApp/useStudentAppHome";
import { useRefetchOnFocus } from "@/hooks/useRefetchOnFocus";
import { translate, translateCount } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { EmptyState } from "@/ui/EmptyState";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { TonalBlock } from "@/ui/TonalBlock";
import { AnimatedListItem } from "@/ui/motion/AnimatedListItem";
import { SkeletonDetail } from "@/ui/motion/SkeletonDetail";

export function StudentAppClassPacksScreen() {
  const router = useRouter();
  const home = useStudentAppHome();
  const classPacks = useStudentAppClassPacks();
  useRefetchOnFocus(classPacks.refetch);
  const students = home.data?.students ?? [];
  const title = translate("student.classes.title");

  if (home.isPending || classPacks.isPending) {
    return (
      <ScrollScreen header={<ScreenHeader navigation="back" title={title} />}>
        <SkeletonDetail />
      </ScrollScreen>
    );
  }
  if (home.isError || classPacks.isError || classPacks.data === undefined) {
    return (
      <ScrollScreen header={<ScreenHeader navigation="back" title={title} />}>
        <Banner message={translate("student.loadError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => {
              void home.refetch();
              void classPacks.refetch();
            }}
          />
        </Banner>
      </ScrollScreen>
    );
  }

  const { unpaidClasses, purchases } = classPacks.data;
  const studentNameOf = (studentId: string | null): string | null => {
    if (students.length < 2) {
      return null;
    }
    const classStudent = students.find((familyStudent) => familyStudent.id === studentId);
    return classStudent === undefined ? null : firstNameOf(classStudent.fullName);
  };
  return (
    <ScrollScreen header={<ScreenHeader navigation="back" title={title} />}>
      {unpaidClasses.length === 0 ? null : (
        <AnimatedListItem index={0} className="gap-2.5">
          <SectionTitle title={translateCount("student.classes.unpaid", unpaidClasses.length)} />
          <TonalBlock tone="danger">
            <View>
              {unpaidClasses.map((unpaidClass, index) => (
                <PackClassRow
                  key={`${unpaidClass.date}-${unpaidClass.className}-${index}`}
                  packClass={unpaidClass}
                  studentName={studentNameOf(unpaidClass.studentId)}
                  hasDivider={index < unpaidClasses.length - 1}
                />
              ))}
            </View>
            <AppText variant="caption" tone="dangerSoft">
              {translate("student.classPacks.unpaidHint")}
            </AppText>
          </TonalBlock>
          <Button
            label={translate("student.classes.buyMore")}
            onPress={() => router.navigate(routes.studentAppShop)}
          />
        </AnimatedListItem>
      )}
      <AnimatedListItem index={1} className="gap-2.5">
        <SectionTitle title={translate("student.classPacks.purchasesTitle")} />
        {purchases.length === 0 ? (
          <EmptyState icon="classPacks" message={translate("student.classPacks.empty")} />
        ) : (
          purchases.map((purchase) => (
            <PackPurchaseCard key={purchase.id} purchase={purchase} studentNameOf={studentNameOf} />
          ))
        )}
      </AnimatedListItem>
    </ScrollScreen>
  );
}
