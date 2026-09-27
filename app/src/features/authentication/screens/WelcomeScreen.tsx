import { useRouter } from "expo-router";
import { ScrollView, View } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { BrandMark } from "@/ui/BrandMark";
import { Button } from "@/ui/Button";

export function WelcomeScreen() {
  const router = useRouter();
  return (
    <SafeAreaView className="flex-1 bg-background">
      <ScrollView contentContainerClassName="w-full max-w-md flex-grow gap-6 self-center px-6 pb-7 pt-5">
        <View className="min-h-[340px] flex-1 items-center justify-center overflow-hidden rounded-[32px] bg-primary-soft">
          <View className="absolute -bottom-10 -left-[10%] -right-[10%] h-[150px] rounded-full bg-primary/20" />
          <View className="absolute -bottom-20 -left-[20%] right-0 h-[150px] rounded-full bg-primary/20" />
          <BrandMark size="large" />
        </View>
        <View className="gap-2.5">
          <AppText variant="hero" accessibilityRole="header">
            {translate("authentication.welcome.title")}
          </AppText>
          <AppText variant="lead" tone="muted">
            {translate("authentication.welcome.subtitle")}
          </AppText>
        </View>
        <View className="gap-2.5">
          <Button
            label={translate("authentication.welcome.signUp")}
            onPress={() => router.push(routes.signUp)}
          />
          <Button
            variant="secondary"
            label={translate("authentication.welcome.signIn")}
            onPress={() => router.push(routes.signIn)}
          />
        </View>
      </ScrollView>
    </SafeAreaView>
  );
}
