import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ScrollScreen } from "@/ui/Screen";
import { HeaderNavigation, ScreenHeader } from "@/ui/ScreenHeader";
import { Spinner } from "@/ui/Spinner";

interface SettingsItemStateProps {
  isPending: boolean;
  isError: boolean;
  notFoundMessage: string;
  onRetry: () => void;
  navigation?: HeaderNavigation;
}

export function SettingsItemState({
  isPending,
  isError,
  notFoundMessage,
  onRetry,
  navigation = "back",
}: SettingsItemStateProps) {
  return (
    <ScrollScreen header={<ScreenHeader navigation={navigation} title="" />}>
      {isPending ? (
        <Spinner size="large" className="mt-6" />
      ) : isError ? (
        <Banner message={translate("common.unexpectedError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={onRetry}
          />
        </Banner>
      ) : (
        <Banner message={notFoundMessage} />
      )}
    </ScrollScreen>
  );
}
