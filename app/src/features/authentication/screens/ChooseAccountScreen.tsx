import { useRouter } from "expo-router";
import { AccountList } from "@/features/authentication/components/AccountList";
import { homeRouteOf } from "@/features/authentication/components/RequireSessionKind";
import { Account } from "@/features/authentication/types";
import { useAccounts } from "@/features/authentication/useAccounts";
import { useOpenAccount } from "@/features/authentication/useOpenAccount";
import { useSession } from "@/features/authentication/useSession";
import { translate } from "@/i18n/translate";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";

export function ChooseAccountScreen() {
  const router = useRouter();
  const { session, confirmAccount } = useSession();
  const { data: accounts, isPending } = useAccounts();
  const isChoosingAtSignIn = session.status === "signedIn" && session.mustChooseAccount === true;
  const { switchingKey, hasFailed, open } = useOpenAccount({
    onCurrent: (account: Account) => {
      confirmAccount();
      router.replace(homeRouteOf(account.kind));
    },
    onSwitched: (account: Account) => router.replace(homeRouteOf(account.kind)),
  });

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
      <AccountList
        accounts={accounts}
        isPending={isPending}
        switchingKey={switchingKey}
        hasFailed={hasFailed}
        onOpen={open}
      />
    </ScrollScreen>
  );
}
