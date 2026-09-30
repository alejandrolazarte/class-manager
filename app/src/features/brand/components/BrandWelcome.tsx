import { Pressable, View } from "react-native";
import { BrandLogo } from "@/features/brand/components/BrandLogo";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";

interface BrandWelcomeProps {
  displayName: string | null;
  logoUri: string | null;
  onDismiss: () => void;
}

export function BrandWelcome({ displayName, logoUri, onDismiss }: BrandWelcomeProps) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={translate("brand.welcome.skip")}
      onPress={onDismiss}
      testID="brand-welcome"
      className={`absolute inset-0 z-20 items-center justify-center gap-4 px-8 ${displayName === null ? "bg-background" : "bg-primary"}`}
    >
      {displayName === null ? null : (
        <>
          <BrandLogo displayName={displayName} logoUri={logoUri} size="large" onPrimary />
          <View className="items-center gap-1.5">
            <AppText variant="display" tone="onPrimary" className="text-center">
              {displayName}
            </AppText>
            <AppText variant="bodyStrong" tone="onPrimary" className="text-center opacity-80">
              {translate("brand.welcome.message")}
            </AppText>
          </View>
        </>
      )}
    </Pressable>
  );
}
