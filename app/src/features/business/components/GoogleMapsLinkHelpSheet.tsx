import { View } from "react-native";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { BottomSheet } from "@/ui/BottomSheet";
import { Button } from "@/ui/Button";
import { NumberedStep } from "@/ui/NumberedStep";

interface GoogleMapsLinkHelpSheetProps {
  onClose: () => void;
}

export function GoogleMapsLinkHelpSheet({ onClose }: GoogleMapsLinkHelpSheetProps) {
  return (
    <BottomSheet onClose={onClose}>
      <AppText variant="headline" accessibilityRole="header">
        {translate("businessLocation.help.title")}
      </AppText>
      <View>
        <NumberedStep
          number={1}
          title={translate("businessLocation.help.searchTitle")}
          detail={translate("businessLocation.help.searchDetail")}
          hasDivider
        />
        <NumberedStep
          number={2}
          title={translate("businessLocation.help.shareTitle")}
          detail={translate("businessLocation.help.shareDetail")}
          hasDivider
        />
        <NumberedStep
          number={3}
          title={translate("businessLocation.help.copyTitle")}
          detail={translate("businessLocation.help.copyDetail")}
        />
      </View>
      <Button
        variant="secondary"
        label={translate("businessLocation.help.done")}
        onPress={onClose}
      />
    </BottomSheet>
  );
}
