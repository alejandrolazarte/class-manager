import { View } from "react-native";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { LoadingScreen } from "@/ui/LoadingScreen";

interface SettingsItemStateProps {
  isPending: boolean;
  isError: boolean;
  notFoundMessage: string;
  onRetry: () => void;
}

export function SettingsItemState({
  isPending,
  isError,
  notFoundMessage,
  onRetry,
}: SettingsItemStateProps) {
  if (isPending) {
    return <LoadingScreen />;
  }
  return (
    <View className="flex-1 bg-background p-4">
      {isError ? (
        <Banner message={translate("common.unexpectedError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={onRetry} />
        </Banner>
      ) : (
        <Banner message={notFoundMessage} />
      )}
    </View>
  );
}
