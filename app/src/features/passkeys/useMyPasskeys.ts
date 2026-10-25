import { useQuery } from "@tanstack/react-query";
import { listMyPasskeys } from "@/features/passkeys/passkeysApi";
import { passkeyQueryKeys } from "@/features/passkeys/passkeyQueryKeys";

export function useMyPasskeys(isEnabled: boolean = true) {
  return useQuery({ queryKey: passkeyQueryKeys.mine, queryFn: listMyPasskeys, enabled: isEnabled });
}
