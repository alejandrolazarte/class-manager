import { View } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useWebAppUpdate } from "@/features/appUpdate/useWebAppUpdate";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Icon } from "@/ui/Icon";

const bannerGapPixels = 8;

function reloadApp() {
  window.location.reload();
}

export function AppUpdateBanner() {
  const isUpdateAvailable = useWebAppUpdate();
  const { top } = useSafeAreaInsets();

  if (!isUpdateAvailable) {
    return null;
  }

  return (
    <View
      pointerEvents="box-none"
      className="absolute left-0 right-0 items-center px-4"
      style={{ top: top + bannerGapPixels }}
    >
      <View
        accessibilityRole="alert"
        className="w-full max-w-[480px] flex-row items-center gap-3 rounded-[18px] bg-inverse py-2 pl-4 pr-2"
      >
        <Icon name="celebration" size="medium" tone="inverse-foreground" />
        <AppText variant="bodyStrong" tone="inverse" className="flex-1 font-label">
          {translate("appUpdate.available")}
        </AppText>
        <Button label={translate("appUpdate.reload")} onPress={reloadApp} size="medium" />
      </View>
    </View>
  );
}
