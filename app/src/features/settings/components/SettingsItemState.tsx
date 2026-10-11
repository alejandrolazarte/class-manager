import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ScrollScreen } from "@/ui/Screen";
import { HeaderNavigation, ScreenHeader } from "@/ui/ScreenHeader";
import { SkeletonDetail } from "@/ui/motion/SkeletonDetail";

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
        <SkeletonDetail />
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
