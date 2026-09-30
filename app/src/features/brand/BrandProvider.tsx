import { useQuery } from "@tanstack/react-query";
import { createContext, PropsWithChildren, useContext, useEffect, useState } from "react";
import { View } from "react-native";
import { getBrand, getBrandLogo } from "@/features/brand/brandApi";
import { brandQueryKeys } from "@/features/brand/brandQueryKeys";
import { toBrandTheme } from "@/features/brand/brandTheme";
import { BrandWelcome } from "@/features/brand/components/BrandWelcome";
import { Brand, BrandAudience } from "@/features/brand/types";
import { useTheme } from "@/theme/useTheme";

export const welcomeDurationInMilliseconds = 1600;

export interface CurrentBrand {
  audience: BrandAudience;
  brand: Brand | null;
  logoUri: string | null;
}

const CurrentBrandContext = createContext<CurrentBrand | null>(null);

interface BrandProviderProps extends PropsWithChildren {
  audience: BrandAudience;
}

export function BrandProvider({ audience, children }: BrandProviderProps) {
  const { setBrandTheme } = useTheme();
  const brandQuery = useQuery({
    queryKey: brandQueryKeys.current(audience),
    queryFn: () => getBrand(audience),
  });
  const brand = brandQuery.data ?? null;
  const logoUpdatedAt = brand?.logoUpdatedAt ?? null;
  const logoQuery = useQuery({
    queryKey: brandQueryKeys.logo(audience, logoUpdatedAt),
    queryFn: () => getBrandLogo(audience),
    enabled: logoUpdatedAt !== null,
    staleTime: Infinity,
  });
  const logoUri = logoUpdatedAt === null ? null : (logoQuery.data ?? null);
  const isSettled = !brandQuery.isPending && (logoUpdatedAt === null || !logoQuery.isPending);
  const [isWelcomeVisible, setIsWelcomeVisible] = useState(true);

  const themeColor = brand?.themeColor ?? null;
  const accentColor = brand?.accentColor ?? null;
  const locksTheme = brand?.locksTheme ?? false;
  useEffect(() => {
    setBrandTheme(toBrandTheme({ themeColor, accentColor, locksTheme }));
  }, [setBrandTheme, themeColor, accentColor, locksTheme]);
  useEffect(() => () => setBrandTheme(null), [setBrandTheme]);

  useEffect(() => {
    if (!isSettled) {
      return;
    }
    const timeout = setTimeout(() => setIsWelcomeVisible(false), welcomeDurationInMilliseconds);
    return () => clearTimeout(timeout);
  }, [isSettled]);

  return (
    <CurrentBrandContext.Provider value={{ audience, brand, logoUri }}>
      <View className="flex-1">
        {children}
        {isWelcomeVisible && !brandQuery.isError ? (
          <BrandWelcome
            displayName={brand?.displayName ?? null}
            logoUri={logoUri}
            onDismiss={() => setIsWelcomeVisible(false)}
          />
        ) : null}
      </View>
    </CurrentBrandContext.Provider>
  );
}

export function useCurrentBrand(): CurrentBrand | null {
  return useContext(CurrentBrandContext);
}
