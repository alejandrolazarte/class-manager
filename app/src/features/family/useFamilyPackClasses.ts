import { useQuery } from "@tanstack/react-query";
import { getFamilyPackClasses } from "@/features/family/familyApi";
import { familyQueryKeys } from "@/features/family/familyQueryKeys";

export function useFamilyPackClasses(studentId: string | null) {
  return useQuery({
    queryKey: familyQueryKeys.packClasses(studentId ?? ""),
    queryFn: () => getFamilyPackClasses(studentId ?? ""),
    enabled: studentId !== null,
  });
}
