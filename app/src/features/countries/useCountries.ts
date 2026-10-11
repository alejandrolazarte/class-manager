import { useQuery } from "@tanstack/react-query";
import { listCountries } from "@/features/countries/countriesApi";

const countriesQueryKey = ["countries"] as const;

export function useCountries() {
  return useQuery({ queryKey: countriesQueryKey, queryFn: listCountries, staleTime: Infinity });
}
