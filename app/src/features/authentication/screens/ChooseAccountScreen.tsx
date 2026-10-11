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
import { HairlineRow } from "@/ui/HairlineRow";
import { Icon } from "@/ui/Icon";
import { IconCircle } from "@/ui/IconCircle";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { Spinner } from "@/ui/Spinner";
import { StatusPill } from "@/ui/StatusPill";

function accountLabel(account: Account): string {
  return translate(account.kind === "student" ? "accounts.asStudent" : "accounts.asTeam", {
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
      <View>
        {(accounts ?? []).map((account) => {
          const key = `${account.kind}-${account.businessId}`;
          return (
            <HairlineRow
              key={key}
              onPress={() => open(account)}
              accessibilityLabel={accountLabel(account)}
            >
              <IconCircle icon={account.kind === "student" ? "home" : "business"} tone="primary" />
              <View className="min-w-0 flex-1 gap-0.5">
                <AppText variant="heading">{account.businessName}</AppText>
                <AppText variant="caption" tone="subtle">
                  {translate(
                    account.kind === "student" ? "accounts.studentHint" : "accounts.teamHint",
                  )}
                </AppText>
              </View>
              {switchingKey === key ? (
                <Spinner />
              ) : account.isCurrent ? (
                <StatusPill isSmall label={translate("accounts.current")} tone="primary" />
              ) : (
                <Icon name="next" size="medium" tone="subtle-foreground" />
              )}
            </HairlineRow>
          );
        })}
      </View>
    </ScrollScreen>
  );
}
