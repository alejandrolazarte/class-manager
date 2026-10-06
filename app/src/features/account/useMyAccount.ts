import { useQuery } from "@tanstack/react-query";
import { getMyAccount } from "@/features/account/accountApi";
import { accountQueryKeys } from "@/features/account/accountQueryKeys";

export function useMyAccount() {
  return useQuery({ queryKey: accountQueryKeys.myAccount, queryFn: getMyAccount });
}
