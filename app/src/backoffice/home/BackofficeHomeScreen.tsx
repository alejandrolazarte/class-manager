import { useQuery } from "@tanstack/react-query";
import { Redirect, useRouter } from "expo-router";
import { View } from "react-native";
import { isBackofficeSessionEnded } from "@/backoffice/api/backofficeHttpClient";
import { formatDateOnly } from "@/backoffice/backofficeFormatting";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { featureLabel, planLabel, statusLabel } from "@/backoffice/brands/brandLabels";
import { brandsQueryKey, expiringAddOnsQueryKey } from "@/backoffice/brands/brandQueryKeys";
import { BrandSummary, getBrands } from "@/backoffice/brands/brandsApi";
import { getExpiringAddOns } from "@/backoffice/brands/expiringAddOnsApi";
import { useBackofficeSession } from "@/backoffice/session/useBackofficeSession";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { CountsRow } from "@/ui/CountsRow";
import { HairlineSection } from "@/ui/HairlineSection";
import { IconButton } from "@/ui/IconButton";
import { ListDivider, ListRow } from "@/ui/ListRow";
import { SkeletonList } from "@/ui/motion/SkeletonList";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";

const trialEndingWithinDays = 7;

function trialsEndingSoon(brands: BrandSummary[]): BrandSummary[] {
  return brands
    .filter(
      (brand) =>
        brand.status === "Trial" &&
        brand.trialDaysLeft !== null &&
        brand.trialDaysLeft <= trialEndingWithinDays,
    )
    .sort((first, second) => (first.trialDaysLeft ?? 0) - (second.trialDaysLeft ?? 0));
}

function countOf(brands: BrandSummary[], status: BrandSummary["status"]): number {
  return brands.filter((brand) => brand.status === status).length;
}

export function BackofficeHomeScreen() {
  const router = useRouter();
  const { session, signOut } = useBackofficeSession();
  const brands = useQuery({ queryKey: brandsQueryKey, queryFn: getBrands, retry: false });
  const expiringAddOns = useQuery({
    queryKey: expiringAddOnsQueryKey,
    queryFn: getExpiringAddOns,
    retry: false,
  });

  if (isBackofficeSessionEnded(brands.error) || isBackofficeSessionEnded(expiringAddOns.error)) {
    return <Redirect href={backofficeRoutes.signIn} />;
  }

  const allBrands = brands.data ?? [];
  const endingTrials = trialsEndingSoon(allBrands);
  const operator = session.data;
  const openBrand = (brandId: string) => router.push(backofficeRoutes.brand(brandId));

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          eyebrow={translate("backoffice.signIn.title")}
          title={translate("backoffice.tabs.home")}
          caption={
            operator
              ? translate("backoffice.home.signedInAs", {
                  email: operator.email,
                  role: translate(`roles.${operator.role}` as TranslationKey),
                })
              : undefined
          }
          accessory={
            <IconButton
              icon="signOut"
              accessibilityLabel={translate("backoffice.home.signOut")}
              onPress={signOut}
            />
          }
        />
      }
    >
      {brands.isError || expiringAddOns.isError ? (
        <Banner message={translate("common.unexpectedError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => {
              void brands.refetch();
              void expiringAddOns.refetch();
            }}
          />
        </Banner>
      ) : null}
      {brands.isPending ? (
        <SkeletonList />
      ) : (
        <>
          <CountsRow
            accessibilityLabel={translate("backoffice.home.countsLabel")}
            items={[
              {
                key: "active",
                count: countOf(allBrands, "Active"),
                label: translate("backoffice.home.activeBrands"),
                tone: "success",
              },
              {
                key: "trial",
                count: countOf(allBrands, "Trial"),
                label: translate("backoffice.home.trialBrands"),
                tone: "default",
              },
              {
                key: "ended",
                count: countOf(allBrands, "Ended"),
                label: translate("backoffice.home.endedBrands"),
                tone: "danger",
              },
            ]}
          />
          <HairlineSection title={translate("backoffice.home.trialsEnding")} isCompact>
            {endingTrials.length === 0 ? (
              <AppText tone="muted">{translate("backoffice.home.noTrialsEnding")}</AppText>
            ) : (
              endingTrials.map((brand, index) => (
                <View key={brand.id}>
                  {index === 0 ? null : <ListDivider />}
                  <ListRow
                    label={brand.name}
                    detail={`${planLabel(brand.planCode)} · ${statusLabel(brand.status, brand.trialDaysLeft)}`}
                    onPress={() => openBrand(brand.id)}
                  />
                </View>
              ))
            )}
          </HairlineSection>
          <HairlineSection title={translate("backoffice.home.addOnsEnding")} isCompact>
            {(expiringAddOns.data ?? []).length === 0 ? (
              <AppText tone="muted">{translate("backoffice.home.noAddOnsEnding")}</AppText>
            ) : (
              (expiringAddOns.data ?? []).map((addOn, index) => (
                <View key={`${addOn.brandId}-${addOn.featureCode}`}>
                  {index === 0 ? null : <ListDivider />}
                  <ListRow
                    label={addOn.brandName}
                    detail={translate("backoffice.home.addOnEndsOn", {
                      feature: featureLabel(addOn.featureCode),
                      date: formatDateOnly(addOn.expiredOn),
                    })}
                    onPress={() => openBrand(addOn.brandId)}
                  />
                </View>
              ))
            )}
          </HairlineSection>
        </>
      )}
    </ScrollScreen>
  );
}
