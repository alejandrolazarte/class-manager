import { ReactElement } from "react";
import { FlatList, View } from "react-native";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { EmptyState } from "@/ui/EmptyState";
import { FloatingActionButton } from "@/ui/FloatingActionButton";
import { IconName } from "@/ui/Icon";
import { Screen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { Spinner } from "@/ui/Spinner";

interface SettingsListScreenLayoutProps<TItem> {
  title: string;
  items: TItem[];
  isPending: boolean;
  isError: boolean;
  isRefetching: boolean;
  onRefetch: () => void;
  keyExtractor: (item: TItem) => string;
  renderItem: (item: TItem) => ReactElement;
  emptyMessage: string;
  emptyIcon: IconName;
  newItemLabel: string;
  onNewItem?: () => void;
}

export function SettingsListScreenLayout<TItem>({
  title,
  items,
  isPending,
  isError,
  isRefetching,
  onRefetch,
  keyExtractor,
  renderItem,
  emptyMessage,
  emptyIcon,
  newItemLabel,
  onNewItem,
}: SettingsListScreenLayoutProps<TItem>) {
  return (
    <Screen
      overlay={
        onNewItem ? <FloatingActionButton label={newItemLabel} onPress={onNewItem} /> : undefined
      }
    >
      <View className="gap-3.5 pb-3.5">
        <ScreenHeader navigation="back" title={title} />
        {isError ? (
          <View className="px-5">
            <Banner message={translate("common.unexpectedError")}>
              <Button
                variant="secondary"
                size="medium"
                label={translate("common.retry")}
                onPress={onRefetch}
              />
            </Banner>
          </View>
        ) : null}
      </View>
      {isPending ? (
        <Spinner className="mt-6" />
      ) : (
        <FlatList
          data={items}
          keyExtractor={keyExtractor}
          renderItem={({ item }) => <View className="px-5 pb-3.5">{renderItem(item)}</View>}
          ListEmptyComponent={
            isError ? null : (
              <EmptyState
                icon={emptyIcon}
                message={emptyMessage}
                createActionLabel={onNewItem ? newItemLabel : undefined}
              />
            )
          }
          refreshing={isRefetching}
          onRefresh={onRefetch}
          contentContainerClassName="w-full max-w-2xl self-center pb-28"
        />
      )}
    </Screen>
  );
}
