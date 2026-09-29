import { useQuery } from "@tanstack/react-query";
import { getFamilyHome } from "@/features/family/familyApi";
import { familyQueryKeys } from "@/features/family/familyQueryKeys";

export function useFamilyHome() {
  return useQuery({ queryKey: familyQueryKeys.home(), queryFn: getFamilyHome });
}
