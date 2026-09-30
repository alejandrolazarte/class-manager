import { useQuery } from "@tanstack/react-query";
import { listAccounts } from "@/features/authentication/authenticationApi";
import { SessionKind } from "@/features/authentication/sessionKind";

export const accountsQueryKey = ["accounts"] as const;

export function useAccounts() {
  return useQuery({ queryKey: accountsQueryKey, queryFn: listAccounts });
}

export function useHasOtherAccountKind(currentKind: SessionKind): boolean {
  const { data: accounts = [] } = useAccounts();
  return accounts.some((account) => account.kind !== currentKind);
}
