import { useQuery } from "@tanstack/react-query";
import { Redirect, useRouter } from "expo-router";
import { isBackofficeSessionEnded } from "@/backoffice/api/backofficeHttpClient";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { planLabel } from "@/backoffice/brands/brandLabels";
import { subscriptionCatalogQueryKey } from "@/backoffice/brands/brandQueryKeys";
import { getSubscriptionCatalog } from "@/backoffice/brands/brandSubscriptionApi";
import { useBackofficeSession } from "@/backoffice/session/useBackofficeSession";
import { formatPercent } from "@/formatting/percent";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { HairlineSection } from "@/ui/HairlineSection";
import { ListRow } from "@/ui/ListRow";
import { SkeletonList } from "@/ui/motion/SkeletonList";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";

export function PlansScreen() {
  const router = useRouter();
  const { canManage } = useBackofficeSession();
  const catalog = useQuery({
    queryKey: subscriptionCatalogQueryKey,
    queryFn: getSubscriptionCatalog,
    retry: false,
  });

  if (isBackofficeSessionEnded(catalog.error)) {
    return <Redirect href={backofficeRoutes.signIn} />;
  }

  return (
    <ScrollScreen header={<ScreenHeader title={translate("backoffice.plans.title")} />}>
      {catalog.isPending ? <SkeletonList /> : null}
      {catalog.isError ? <Banner message={translate("common.unexpectedError")} /> : null}
      {catalog.data === undefined ? null : (
        <HairlineSection title={translate("backoffice.plans.salesFeeTitle")} isCompact>
          <AppText variant="body" tone="muted">
            {translate("backoffice.plans.salesFeeExplanation")}
          </AppText>
          {catalog.data.plans.map((plan) => (
            <ListRow
              key={plan.code}
              label={planLabel(plan.code)}
              detail={formatPercent(plan.salesFeePercent)}
              trailing={
                canManage ? (
                  <Button
                    variant="secondary"
                    size="small"
                    label={translate("backoffice.plans.changeSalesFee")}
                    accessibilityLabel={translate("backoffice.plans.changeSalesFeeOf", {
                      plan: planLabel(plan.code),
                    })}
                    onPress={() => router.push(backofficeRoutes.planSalesFee(plan.code))}
                  />
                ) : undefined
              }
            />
          ))}
        </HairlineSection>
      )}
    </ScrollScreen>
  );
}
