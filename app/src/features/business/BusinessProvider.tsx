import { useQuery } from "@tanstack/react-query";
import { PropsWithChildren } from "react";
import { View } from "react-native";
import { CurrentBusinessProvider } from "@/features/business/CurrentBusinessProvider";
import { getCurrentBusiness } from "@/features/business/businessApi";
import { businessQueryKeys } from "@/features/business/businessQueryKeys";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { LoadingScreen } from "@/ui/LoadingScreen";

export function BusinessProvider({ children }: PropsWithChildren) {
  const businessQuery = useQuery({
    queryKey: businessQueryKeys.current(),
    queryFn: getCurrentBusiness,
  });

  if (businessQuery.data) {
    return (
      <CurrentBusinessProvider business={businessQuery.data}>{children}</CurrentBusinessProvider>
    );
  }
  if (businessQuery.isError) {
    return (
      <View className="flex-1 justify-center bg-gray-50 p-6">
        <Banner message={translate("business.loadError")}>
          <Button
            variant="secondary"
            label={translate("common.retry")}
            onPress={() => businessQuery.refetch()}
          />
        </Banner>
      </View>
    );
  }
  return <LoadingScreen />;
}
