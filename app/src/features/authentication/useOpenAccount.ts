import { useState } from "react";
import { Account } from "@/features/authentication/types";
import { useSession } from "@/features/authentication/useSession";

interface OpenAccountHandlers {
  onCurrent: (account: Account) => void;
  onSwitched: (account: Account) => void;
}

export function accountKeyOf(account: Account): string {
  return `${account.kind}-${account.businessId}`;
}

export function useOpenAccount({ onCurrent, onSwitched }: OpenAccountHandlers) {
  const { switchAccount } = useSession();
  const [switchingKey, setSwitchingKey] = useState<string | null>(null);
  const [hasFailed, setHasFailed] = useState(false);

  const open = async (account: Account) => {
    if (switchingKey !== null) {
      return;
    }
    if (account.isCurrent) {
      onCurrent(account);
      return;
    }
    setHasFailed(false);
    setSwitchingKey(accountKeyOf(account));
    try {
      await switchAccount(account.businessId, account.kind);
      onSwitched(account);
    } catch {
      setHasFailed(true);
    } finally {
      setSwitchingKey(null);
    }
  };

  return { switchingKey, hasFailed, open };
}
