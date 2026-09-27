import { ReactElement } from "react";
import { FlatList, View } from "react-native";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { FloatingActionButton } from "@/ui/FloatingActionButton";
import { AppText } from "@/ui/AppText";
import { Spinner } from "@/ui/Spinner";

interface SettingsListScreenLayoutProps<TItem> {
  items: TItem[];
  isPending: boolean;
  isError: boolean;
  isRefetching: boolean;
  onRefetch: () => void;
  keyExtractor: (item: TItem) => string;
  renderItem: (item: TItem) => ReactElement;
  emptyMessage: string;
  newItemLabel: string;
  onNewItem: () => void;
}

export function SettingsListScreenLayout<TItem>({
  items,
  isPending,
  isError,
  isRefetching,
  onRefetch,
  keyExtractor,
  renderItem,
  emptyMessage,
  newItemLabel,
  onNewItem,
}: SettingsListScreenLayoutProps<TItem>) {
  return (
    <View className="flex-1 bg-background">
      {isError ? (
        <View className="p-4">
          <Banner message={translate("common.unexpectedError")}>
            <Button variant="secondary" label={translate("common.retry")} onPress={onRefetch} />
          </Banner>
        </View>
      ) : null}
      {isPending ? (
        <Spinner className="mt-6" />
      ) : (
        <FlatList
          data={items}
          keyExtractor={keyExtractor}
          renderItem={({ item }) => renderItem(item)}
          ListEmptyComponent={
            isError ? null : (
              <View className="items-center gap-4 p-6">
                <AppText tone="muted" variant="lead">
                  {emptyMessage}
                </AppText>
                <Button label={newItemLabel} onPress={onNewItem} />
              </View>
            )
          }
          refreshing={isRefetching}
          onRefresh={onRefetch}
          contentContainerClassName="pb-24"
        />
      )}
      <FloatingActionButton accessibilityLabel={newItemLabel} onPress={onNewItem} />
    </View>
  );
}
