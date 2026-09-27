import { useRouter } from "expo-router";
import { useState } from "react";
import { FlatList, View } from "react-native";
import { StudentListItem } from "@/features/students/components/StudentListItem";
import { useStudentSearch } from "@/features/students/useStudentSearch";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { FloatingActionButton } from "@/ui/FloatingActionButton";
import { SearchInput } from "@/ui/SearchInput";
import { AppText } from "@/ui/AppText";
import { Spinner } from "@/ui/Spinner";

export function StudentListScreen() {
  const router = useRouter();
  const [searchText, setSearchText] = useState("");
  const {
    data: students = [],
    isPending,
    isError,
    refetch,
    isRefetching,
    debouncedSearch,
  } = useStudentSearch(searchText);
  const openRegisterClient = () => router.push(routes.registerClient);

  const emptyState =
    debouncedSearch.length > 0 ? (
      <AppText variant="body" tone="muted" className="p-6 text-center">
        {translate("students.list.noResults", { search: debouncedSearch })}
      </AppText>
    ) : (
      <View className="items-center gap-4 p-6">
        <AppText tone="muted" variant="lead">
          {translate("students.list.emptyTitle")}
        </AppText>
        <Button label={translate("students.list.emptyCallToAction")} onPress={openRegisterClient} />
      </View>
    );

  return (
    <View className="flex-1 bg-background">
      <View className="p-4">
        <SearchInput
          value={searchText}
          onChangeText={setSearchText}
          placeholder={translate("students.list.searchPlaceholder")}
        />
      </View>
      {isError ? (
        <View className="px-4">
          <Banner message={translate("common.unexpectedError")}>
            <Button
              variant="secondary"
              label={translate("common.retry")}
              onPress={() => refetch()}
            />
          </Banner>
        </View>
      ) : null}
      {isPending ? (
        <Spinner className="mt-6" />
      ) : (
        <FlatList
          data={students}
          keyExtractor={(student) => student.id}
          renderItem={({ item: student }) => (
            <StudentListItem
              student={student}
              onPress={() => router.push(routes.clientDetail(student.clientId))}
            />
          )}
          ListEmptyComponent={isError ? null : emptyState}
          refreshing={isRefetching}
          onRefresh={() => refetch()}
          contentContainerClassName="pb-24"
        />
      )}
      <FloatingActionButton
        accessibilityLabel={translate("students.list.newStudent")}
        onPress={openRegisterClient}
      />
    </View>
  );
}
