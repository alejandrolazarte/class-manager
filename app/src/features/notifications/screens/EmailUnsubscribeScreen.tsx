import { useLocalSearchParams, useRouter } from "expo-router";
import { useEffect, useState } from "react";
import { View } from "react-native";
import { AuthenticationScreenLayout } from "@/features/authentication/components/AuthenticationScreenLayout";
import {
  EmailUnsubscription,
  getEmailUnsubscription,
  resubscribeToEmail,
  unsubscribeFromEmail,
} from "@/features/notifications/emailUnsubscribeApi";
import { translate, TranslationKey } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { BrandMark } from "@/ui/BrandMark";
import { Button } from "@/ui/Button";
import { SkeletonDetail } from "@/ui/motion/SkeletonDetail";
import { useToast } from "@/ui/ToastProvider";

type UnsubscribeState =
  | { kind: "loading" }
  | { kind: "invalidLink" }
  | { kind: "ready"; unsubscription: EmailUnsubscription };

function categoryName(unsubscription: EmailUnsubscription): string {
  return translate(`notificationPreferences.category.${unsubscription.category}` as TranslationKey);
}

export function EmailUnsubscribeScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const { token } = useLocalSearchParams<{ token?: string }>();
  const [state, setState] = useState<UnsubscribeState>(
    token ? { kind: "loading" } : { kind: "invalidLink" },
  );
  const [isSaving, setIsSaving] = useState(false);

  useEffect(() => {
    if (!token) {
      return;
    }
    getEmailUnsubscription(token)
      .then((unsubscription) => setState({ kind: "ready", unsubscription }))
      .catch(() => setState({ kind: "invalidLink" }));
  }, [token]);

  const save = async (
    change: (linkToken: string) => Promise<EmailUnsubscription>,
    message?: string,
  ) => {
    if (!token) {
      return;
    }
    setIsSaving(true);
    try {
      setState({ kind: "ready", unsubscription: await change(token) });
      if (message) {
        showToast(message);
      }
    } catch {
      showToast(translate("common.unexpectedError"));
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <AuthenticationScreenLayout
      title={translate("unsubscribe.title")}
      onBack={() => router.replace(routes.appEntry)}
      mark={<BrandMark />}
    >
      {state.kind === "loading" ? <SkeletonDetail /> : null}
      {state.kind === "invalidLink" ? (
        <Banner message={translate("unsubscribe.invalidLink")} />
      ) : null}
      {state.kind === "ready" && !state.unsubscription.isUnsubscribed ? (
        <View className="gap-4">
          <AppText variant="heading">
            {translate("unsubscribe.question", { category: categoryName(state.unsubscription) })}
          </AppText>
          <AppText variant="body" tone="muted">
            {translate("unsubscribe.description", {
              business: state.unsubscription.businessName,
              description: translate(
                `notificationPreferences.description.${state.unsubscription.category}` as TranslationKey,
              ),
            })}
          </AppText>
          <Button
            label={translate("unsubscribe.confirm")}
            isLoading={isSaving}
            onPress={() => save(unsubscribeFromEmail)}
          />
        </View>
      ) : null}
      {state.kind === "ready" && state.unsubscription.isUnsubscribed ? (
        <View className="gap-4">
          <Banner
            tone="success"
            message={translate("unsubscribe.done", {
              category: categoryName(state.unsubscription),
              business: state.unsubscription.businessName,
            })}
          />
          <Button
            variant="secondary"
            label={translate("unsubscribe.resubscribe")}
            isLoading={isSaving}
            onPress={() => save(resubscribeToEmail, translate("unsubscribe.resubscribed"))}
          />
          <AppText variant="body" tone="muted">
            {translate("unsubscribe.chooseMore")}
          </AppText>
        </View>
      ) : null}
    </AuthenticationScreenLayout>
  );
}
