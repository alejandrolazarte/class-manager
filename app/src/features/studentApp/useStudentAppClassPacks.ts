import { useQuery } from "@tanstack/react-query";
import { getStudentAppClassPacks } from "@/features/studentApp/studentAppApi";
import { studentAppQueryKeys } from "@/features/studentApp/studentAppQueryKeys";

export function useStudentAppClassPacks() {
  return useQuery({
    queryKey: studentAppQueryKeys.classPacks(),
    queryFn: getStudentAppClassPacks,
  });
}
