import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { searchStudents } from "@/features/students/studentsApi";
import { studentQueryKeys } from "@/features/students/studentQueryKeys";
import { useDebouncedValue } from "@/hooks/useDebouncedValue";

export const studentSearchDebounceMilliseconds = 300;
export const studentSearchLimit = 20;

export function useStudentSearch(searchText: string) {
  const debouncedSearch = useDebouncedValue(searchText.trim(), studentSearchDebounceMilliseconds);
  const searchQuery = useQuery({
    queryKey: studentQueryKeys.search(debouncedSearch),
    queryFn: () => searchStudents({ search: debouncedSearch, limit: studentSearchLimit }),
    placeholderData: keepPreviousData,
  });
  return { ...searchQuery, debouncedSearch };
}
