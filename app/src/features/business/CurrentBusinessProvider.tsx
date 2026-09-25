import { createContext, PropsWithChildren, useContext } from "react";
import { Business } from "@/features/business/types";

const CurrentBusinessContext = createContext<Business | null>(null);

interface CurrentBusinessProviderProps extends PropsWithChildren {
  business: Business;
}

export function CurrentBusinessProvider({ business, children }: CurrentBusinessProviderProps) {
  return (
    <CurrentBusinessContext.Provider value={business}>{children}</CurrentBusinessContext.Provider>
  );
}

export function useCurrentBusiness(): Business {
  const business = useContext(CurrentBusinessContext);
  if (business === null) {
    throw new Error("useCurrentBusiness must be used inside CurrentBusinessProvider");
  }
  return business;
}

export function useBusinessTimeZone(): string {
  return useCurrentBusiness().timeZoneId;
}

export function useBusinessCurrency(): string {
  return useCurrentBusiness().currencyCode;
}
