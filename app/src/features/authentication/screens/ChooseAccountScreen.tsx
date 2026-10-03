import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { homeRouteOf } from "@/features/authentication/components/RequireSessionKind";
import { Account } from "@/features/authentication/types";
import { useAccounts } from "@/features/authentication/useAccounts";
import { useSession } from "@/features/authentication/useSession";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { Spinner } from "@/ui/Spinner";
import { StatusPill } from "@/ui/StatusPill";

function accountLabel(account: Account): string {
  return translate(account.kind === "family" ? "accounts.asFamily" : "accounts.asTeam", {
    name: account.businessName,
  });
}

export function ChooseAccountScreen() {
  const router = useRouter();
  const { session, switchAccount, confirmAccount } = useSession();
  const { data: accounts, isPending } = useAccounts();
  const [switchingKey, setSwitchingKey] = useState<string | null>(null);
  const [hasFailed, setHasFailed] = useState(false);
  const isChoosingAtSignIn = session.status === "signedIn" && session.mustChooseAccount === true;

  const open = async (account: Account) => {
    if (switchingKey !== null) {
      return;
    }
    if (account.isCurrent) {
      confirmAccount();
      router.replace(homeRouteOf(account.kind));
      return;
    }
    setHasFailed(false);
    setSwitchingKey(`${account.kind}-${account.businessId}`);
    try {
      await switchAccount(account.businessId, account.kind);
      router.replace(homeRouteOf(account.kind));
    } catch {
      setHasFailed(true);
    } finally {
      setSwitchingKey(null);
    }
  };

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation={isChoosingAtSignIn ? undefined : "back"}
          title={translate(isChoosingAtSignIn ? "accounts.chooseTitle" : "accounts.switch")}
          subtitle={translate("accounts.chooseSubtitle")}
        />
      }
    >
      {hasFailed ? <Banner message={translate("accounts.switchFailed")} /> : null}
      {isPending ? <Spinner className="mt-6" /> : null}
      {(accounts ?? []).map((account) => {
        const key = `${account.kind}-${account.businessId}`;
        return (
          <Card
            key={key}
            onPress={() => open(account)}
            accessibilityLabel={accountLabel(account)}
            className="flex-row items-center gap-3.5 p-4"
          >
            <View className="h-11 w-11 items-center justify-center rounded-full bg-primary-soft">
              <Icon name={account.kind === "family" ? "home" : "business"} tone="primary" />
            </View>
            <View className="min-w-0 flex-1 gap-0.5">
              <AppText variant="heading">{account.businessName}</AppText>
              <AppText variant="caption" tone="subtle">
                {translate(account.kind === "family" ? "accounts.familyHint" : "accounts.teamHint")}
              </AppText>
            </View>
            {switchingKey === key ? (
              <Spinner />
            ) : account.isCurrent ? (
              <StatusPill label={translate("accounts.current")} tone="primary" />
            ) : (
              <Icon name="next" tone="subtle-foreground" />
            )}
          </Card>
        );
      })}
    </ScrollScreen>
  );
}
