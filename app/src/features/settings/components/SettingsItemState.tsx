import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { Spinner } from "@/ui/Spinner";

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
  return (
    <ScrollScreen header={<ScreenHeader navigation="back" title="" />}>
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
