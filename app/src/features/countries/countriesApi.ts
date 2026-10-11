import { httpClient } from "@/api/httpClient";
import { Country } from "@/features/countries/types";

const countriesPath = "/api/countries";

export function listCountries(): Promise<Country[]> {
  return httpClient.get<Country[]>(countriesPath);
}
