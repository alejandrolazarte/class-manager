import { useRouter } from "expo-router";
import { useState } from "react";
import { FlatList, View } from "react-native";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { StudentListItem } from "@/features/students/components/StudentListItem";
import { useStudentSearch } from "@/features/students/useStudentSearch";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { EmptyState } from "@/ui/EmptyState";
import { FloatingActionButton } from "@/ui/FloatingActionButton";
import { Screen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SearchInput } from "@/ui/SearchInput";
import { PlanUsage } from "@/features/subscriptions/components/PlanUsage";
import { usePlanAllowing } from "@/features/subscriptions/PlanOfferProvider";
import { featureCodes } from "@/features/subscriptions/subscriptionCodes";
import { useIsAtLimit } from "@/features/subscriptions/useSubscription";
import { SkeletonList } from "@/ui/motion/SkeletonList";
import { AnimatedListItem } from "@/ui/motion/AnimatedListItem";

export function StudentListScreen() {
  const router = useRouter();
  const isAtStudentLimit = useIsAtLimit(featureCodes.students);
  const canManageStudents = useCan(permissions.studentsManage);
  const withinStudentPlan = usePlanAllowing(featureCodes.students, !isAtStudentLimit);
  const [searchText, setSearchText] = useState("");
  const {
    data: students = [],
    isPending,
    isError,
    refetch,
    isRefetching,
    debouncedSearch,
  } = useStudentSearch(searchText);
  const openRegisterClient = withinStudentPlan(() => router.push(routes.registerClient));

  const emptyState =
    debouncedSearch.length > 0 ? (
      <EmptyState
        icon="search"
        message={translate("students.list.noResults", { search: debouncedSearch })}
      />
    ) : (
      <EmptyState
        icon="students"
        message={translate("students.list.emptyTitle")}
        createActionLabel={canManageStudents ? translate("students.list.newStudent") : undefined}
      />
    );

  return (
    <Screen
      overlay={
        canManageStudents ? (
          <FloatingActionButton
            label={translate("students.list.newStudent")}
            onPress={openRegisterClient}
          />
        ) : undefined
      }
    >
      <View className="gap-3.5 pb-2">
        <ScreenHeader title={translate("students.list.title")} />
        <View className="gap-3.5 px-5">
          <SearchInput
            value={searchText}
            onChangeText={setSearchText}
            placeholder={translate("students.list.searchPlaceholder")}
          />
          {canManageStudents ? <PlanUsage featureCode={featureCodes.students} /> : null}
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
      {isPending ? (
        <SkeletonList className="px-5" />
      ) : (
        <FlatList
          data={students}
          keyExtractor={(student) => student.id}
          keyboardShouldPersistTaps="handled"
          renderItem={({ item: student, index }) => (
            <AnimatedListItem index={index} className="px-5">
              <StudentListItem
                student={student}
                onPress={() => router.push(routes.clientDetail("students", student.clientId))}
              />
            </AnimatedListItem>
          )}
          ListEmptyComponent={isError ? null : emptyState}
          refreshing={isRefetching}
          onRefresh={() => refetch()}
          contentContainerClassName="w-full max-w-2xl self-center pb-28"
        />
      )}
    </Screen>
  );
}
