import { useRouter } from "expo-router";
import { useState } from "react";
import { AccountList } from "@/features/authentication/components/AccountList";
import { homeRouteOf } from "@/features/authentication/components/RequireSessionKind";
import { Account } from "@/features/authentication/types";
import { useAccounts } from "@/features/authentication/useAccounts";
import { useOpenAccount } from "@/features/authentication/useOpenAccount";
import { CurrentBrandLogo } from "@/features/brand/components/CurrentBrandLogo";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { BottomSheet } from "@/ui/BottomSheet";
import { RingButton } from "@/ui/RingButton";
import { useToast } from "@/ui/ToastProvider";

interface AccountSwitcherProps {
  displayName: string;
}

const minimumAccountsToSwitch = 2;

function AccountSwitcherSheet({ onClose }: { onClose: () => void }) {
  const router = useRouter();
  const { showToast } = useToast();
  const { data: accounts, isPending } = useAccounts();
  const { switchingKey, hasFailed, open } = useOpenAccount({
    onCurrent: onClose,
    onSwitched: (account: Account) => {
      onClose();
      showToast(translate("branches.switched", { name: account.businessName }));
      router.replace(homeRouteOf(account.kind));
    },
  });

  return (
    <BottomSheet onClose={onClose}>
      <AppText variant="headline" accessibilityRole="header">
        {translate("accounts.switch")}
      </AppText>
      <AccountList
        accounts={accounts}
        isPending={isPending}
        switchingKey={switchingKey}
        hasFailed={hasFailed}
        onOpen={open}
      />
    </BottomSheet>
  );
}

export function AccountSwitcher({ displayName }: AccountSwitcherProps) {
  const { data: accounts = [] } = useAccounts();
  const [isOpen, setIsOpen] = useState(false);

  if (accounts.length < minimumAccountsToSwitch) {
    return <CurrentBrandLogo />;
  }
  return (
    <>
      <RingButton accessibilityLabel={translate("accounts.switch")} onPress={() => setIsOpen(true)}>
        <CurrentBrandLogo fallbackName={displayName} />
      </RingButton>
      {isOpen ? <AccountSwitcherSheet onClose={() => setIsOpen(false)} /> : null}
    </>
  );
}
