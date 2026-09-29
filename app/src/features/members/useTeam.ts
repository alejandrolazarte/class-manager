import { useQuery } from "@tanstack/react-query";
import { getTeam } from "@/features/members/membersApi";
import { memberQueryKeys } from "@/features/members/memberQueryKeys";

export function useTeam() {
  return useQuery({ queryKey: memberQueryKeys.team(), queryFn: getTeam });
}
