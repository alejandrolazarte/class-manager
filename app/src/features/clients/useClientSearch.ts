import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { clientQueryKeys } from "@/features/clients/clientQueryKeys";
import { searchClients } from "@/features/clients/clientsApi";
import { useDebouncedValue } from "@/hooks/useDebouncedValue";

export const clientSearchDebounceMilliseconds = 300;
export const clientSearchLimit = 20;

export function useClientSearch(searchText: string) {
  const debouncedSearch = useDebouncedValue(searchText.trim(), clientSearchDebounceMilliseconds);
  const searchQuery = useQuery({
    queryKey: clientQueryKeys.search(debouncedSearch),
    queryFn: () => searchClients({ search: debouncedSearch, limit: clientSearchLimit }),
    placeholderData: keepPreviousData,
  });
  return { ...searchQuery, debouncedSearch };
}
