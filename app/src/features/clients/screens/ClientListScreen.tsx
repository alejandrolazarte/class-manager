import { useRouter } from "expo-router";
import { useState } from "react";
import { ActivityIndicator, FlatList, Text, View } from "react-native";
import { ClientListItem } from "@/features/clients/components/ClientListItem";
import { ClientSearchInput } from "@/features/clients/components/ClientSearchInput";
import { useClientSearch } from "@/features/clients/useClientSearch";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { FloatingActionButton } from "@/ui/FloatingActionButton";

export function ClientListScreen() {
  const router = useRouter();
  const [searchText, setSearchText] = useState("");
  const {
    data: clients = [],
    isPending,
    isError,
    refetch,
    isRefetching,
    debouncedSearch,
  } = useClientSearch(searchText);
  const openNewClient = () => router.push(routes.newClient);

  const emptyState =
    debouncedSearch.length > 0 ? (
      <Text className="p-6 text-center text-base text-gray-600">
        {translate("clients.list.noResults", { search: debouncedSearch })}
      </Text>
    ) : (
      <View className="items-center gap-4 p-6">
        <Text className="text-lg text-gray-700">{translate("clients.list.emptyTitle")}</Text>
        <Button label={translate("clients.list.emptyCallToAction")} onPress={openNewClient} />
      </View>
    );

  return (
    <View className="flex-1 bg-gray-50">
      <View className="p-4">
        <ClientSearchInput
          value={searchText}
          onChangeText={setSearchText}
          placeholder={translate("clients.list.searchPlaceholder")}
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
          data={clients}
          keyExtractor={(client) => client.id}
          renderItem={({ item: client }) => (
            <ClientListItem
              client={client}
              onPress={() => router.push(routes.clientDetail(client.id))}
            />
          )}
          ListEmptyComponent={isError ? null : emptyState}
          refreshing={isRefetching}
          onRefresh={() => refetch()}
          contentContainerClassName="pb-24"
        />
      )}
      <FloatingActionButton
        accessibilityLabel={translate("clients.list.newClient")}
        onPress={openNewClient}
      />
    </View>
  );
}
