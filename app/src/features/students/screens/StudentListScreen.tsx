import { useRouter } from "expo-router";
import { useState } from "react";
import { ActivityIndicator, FlatList, Text, View } from "react-native";
import { StudentListItem } from "@/features/students/components/StudentListItem";
import { useStudentSearch } from "@/features/students/useStudentSearch";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { FloatingActionButton } from "@/ui/FloatingActionButton";
import { SearchInput } from "@/ui/SearchInput";

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
      <Text className="p-6 text-center text-base text-gray-600">
        {translate("students.list.noResults", { search: debouncedSearch })}
      </Text>
    ) : (
      <View className="items-center gap-4 p-6">
        <Text className="text-lg text-gray-700">{translate("students.list.emptyTitle")}</Text>
        <Button label={translate("students.list.emptyCallToAction")} onPress={openRegisterClient} />
      </View>
    );

  return (
    <View className="flex-1 bg-gray-50">
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
        <ActivityIndicator className="mt-6" />
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
        accessibilityLabel={translate("students.list.newClient")}
        onPress={openRegisterClient}
      />
    </View>
  );
}
