import { useQuery } from "@tanstack/react-query";
import { getFamilyMakeups } from "@/features/family/familyApi";
import { familyQueryKeys } from "@/features/family/familyQueryKeys";

export function useFamilyMakeups(studentId: string | null) {
  return useQuery({
    queryKey: familyQueryKeys.makeups(studentId ?? ""),
    queryFn: () => getFamilyMakeups(studentId ?? ""),
    enabled: studentId !== null,
  });
}
