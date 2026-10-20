import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Redirect, useRouter } from "expo-router";
import { View } from "react-native";
import {
  isBackofficeNotFound,
  isBackofficeSessionEnded,
} from "@/backoffice/api/backofficeHttpClient";
import { formatDateOnly, formatDateTimeAsDate } from "@/backoffice/backofficeFormatting";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { featureLabel, planLabel, statusLabel, statusTone } from "@/backoffice/brands/brandLabels";
import { brandQueryKey, brandsQueryKey } from "@/backoffice/brands/brandQueryKeys";
import { BrandAddOn, BrandDetail, BrandFeature, getBrand } from "@/backoffice/brands/brandsApi";
import { removeBrandAddOn, restoreBrandAddOn } from "@/backoffice/brands/brandSubscriptionApi";
import { useBackofficeSession } from "@/backoffice/session/useBackofficeSession";
import { formatMoney } from "@/formatting/money";
import { formatMegabytes } from "@/formatting/storageSize";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { HairlineSection } from "@/ui/HairlineSection";
import { IconButton } from "@/ui/IconButton";
import { ListDivider, ListRow } from "@/ui/ListRow";
import { SkeletonDetail } from "@/ui/motion/SkeletonDetail";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { StatusPill } from "@/ui/StatusPill";
import { useToast } from "@/ui/ToastProvider";

const branchesFeatureCode = "branches";
const documentStorageFeatureCode = "document-storage";

interface BrandScreenProps {
  brandId: string;
}

function featureText(feature: BrandFeature): string {
  const name = featureLabel(feature.code);
  return feature.limit === null
    ? name
    : translate("subscriptions.featureWithLimit", {
        feature: name,
        limit:
          feature.code === documentStorageFeatureCode
            ? formatMegabytes(feature.limit)
            : feature.limit,
      });
}

function branchesHint(brand: BrandDetail): string | undefined {
  const branchesFeature = brand.features.find((feature) => feature.code === branchesFeatureCode);
  if (branchesFeature === undefined) {
    return undefined;
  }
  const values = { feature: featureLabel(branchesFeatureCode), used: brand.branches.length };
  return branchesFeature.limit === null
    ? translate("subscriptions.featureUsedUnlimited", values)
    : translate("subscriptions.featureUsed", { ...values, limit: branchesFeature.limit });
}

interface BrandSectionsProps {
  brand: BrandDetail;
  canManage: boolean;
}

function useAddOnRemoval(brandId: string) {
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const refreshBrand = () => queryClient.invalidateQueries({ queryKey: brandsQueryKey });
  const restore = useMutation({
    mutationFn: (addOnId: string) => restoreBrandAddOn(brandId, addOnId),
    onSettled: refreshBrand,
    onError: () => showToast(translate("common.unexpectedError")),
  });
  const remove = useMutation({
    mutationFn: (addOn: BrandAddOn) => removeBrandAddOn(brandId, addOn.id),
    onSettled: refreshBrand,
    onSuccess: (removedAddOn) =>
      showToast(translate("backoffice.brand.addOnRemoved"), {
        label: translate("common.undo"),
        onPress: () => restore.mutate(removedAddOn.id),
      }),
    onError: () => showToast(translate("common.unexpectedError")),
  });
  return remove;
}

function BrandSections({ brand, canManage }: BrandSectionsProps) {
  const router = useRouter();
  const removeAddOn = useAddOnRemoval(brand.id);
  const subscription = brand.subscription;
  return (
    <>
      <HairlineSection title={translate("backoffice.brand.ownersTitle")} isCompact>
        {brand.owners.map((owner) => (
          <ListRow
            key={`${owner.fullName}-${owner.email ?? ""}`}
            label={owner.fullName}
            detail={owner.email ?? undefined}
          />
        ))}
      </HairlineSection>
      <HairlineSection
        title={translate("backoffice.brand.subscriptionTitle")}
        trailing={
          <StatusPill
            label={statusLabel(brand.status, subscription?.trialDaysLeft ?? null)}
            tone={statusTone(brand.status)}
            isSmall
          />
        }
        isCompact
      >
        {subscription === null ? (
          <AppText tone="muted">{translate("backoffice.brand.noSubscription")}</AppText>
        ) : (
          <View className="gap-0.5">
            <AppText variant="bodyStrong">
              {`${planLabel(subscription.planCode)} · ${translate("subscriptions.pricePerMonth", {
                price: formatMoney(subscription.price, subscription.currency),
              })}`}
            </AppText>
            <AppText variant="caption" tone="muted">
              {translate("backoffice.brand.since", {
                date: formatDateTimeAsDate(subscription.createdOn),
              })}
              {subscription.expiredOn === null
                ? ""
                : ` · ${translate("backoffice.brand.until", { date: formatDateOnly(subscription.expiredOn) })}`}
            </AppText>
            {subscription.note === null ? null : (
              <AppText variant="caption" tone="subtle">
                {subscription.note}
              </AppText>
            )}
          </View>
        )}
        {canManage ? (
          <View className="flex-row">
            <Button
              variant="secondary"
              size="small"
              label={translate("backoffice.brand.changePlan")}
              onPress={() => router.push(backofficeRoutes.changePlan(brand.id))}
            />
          </View>
        ) : null}
      </HairlineSection>
      {brand.features.length === 0 ? null : (
        <HairlineSection title={translate("backoffice.brand.featuresTitle")} isCompact>
          {brand.features.map((feature) => (
            <AppText key={feature.code} variant="caption">
              {featureText(feature)}
            </AppText>
          ))}
        </HairlineSection>
      )}
      <HairlineSection
        title={translate("backoffice.brand.branchesTitle")}
        hint={branchesHint(brand)}
        isCompact
      >
        {brand.branches.map((branch, index) => (
          <View key={branch.id}>
            {index === 0 ? null : <ListDivider />}
            <ListRow
              label={branch.name}
              detail={translate("backoffice.brand.branchUsage", {
                students: branch.studentCount,
                team: branch.teamCount,
                storage: formatMegabytes(branch.storageMegabytes),
              })}
            />
          </View>
        ))}
      </HairlineSection>
      <HairlineSection title={translate("backoffice.brand.addOnsTitle")} isCompact>
        {brand.addOns.length === 0 ? (
          <AppText tone="muted">{translate("backoffice.brand.noAddOns")}</AppText>
        ) : (
          brand.addOns.map((addOn) => (
            <ListRow
              key={addOn.id}
              label={featureText({ code: addOn.featureCode, limit: addOn.limit })}
              detail={translate("backoffice.brand.addOnDetail", {
                price: formatMoney(addOn.price, addOn.currency),
                date: formatDateTimeAsDate(addOn.createdOn),
              })}
              trailing={
                canManage ? (
                  <IconButton
                    icon="delete"
                    tone="muted-foreground"
                    accessibilityLabel={translate("backoffice.brand.removeAddOn", {
                      feature: featureLabel(addOn.featureCode),
                    })}
                    onPress={() => removeAddOn.mutate(addOn)}
                    disabled={removeAddOn.isPending}
                  />
                ) : undefined
              }
            />
          ))
        )}
        {canManage && subscription !== null ? (
          <Button
            variant="dashed"
            label={translate("backoffice.brand.giveAddOn")}
            onPress={() => router.push(backofficeRoutes.giveAddOn(brand.id))}
          />
        ) : null}
      </HairlineSection>
      <HairlineSection title={translate("backoffice.brand.historyTitle")} isCompact>
        {brand.pastSubscriptions.length === 0 ? (
          <AppText tone="muted">{translate("backoffice.brand.noHistory")}</AppText>
        ) : (
          brand.pastSubscriptions.map((pastSubscription) => (
            <ListRow
              key={pastSubscription.createdOn}
              label={planLabel(pastSubscription.planCode)}
              detail={translate("backoffice.brand.pastPeriod", {
                price: formatMoney(pastSubscription.price, pastSubscription.currency),
                from: formatDateTimeAsDate(pastSubscription.createdOn),
                to: formatDateTimeAsDate(pastSubscription.deletedAt ?? pastSubscription.createdOn),
              })}
            />
          ))
        )}
      </HairlineSection>
    </>
  );
}

export function BrandScreen({ brandId }: BrandScreenProps) {
  const router = useRouter();
  const { canManage } = useBackofficeSession();
  const brand = useQuery({
    queryKey: brandQueryKey(brandId),
    queryFn: () => getBrand(brandId),
    retry: false,
  });

  if (isBackofficeSessionEnded(brand.error)) {
    return <Redirect href={backofficeRoutes.signIn} />;
  }

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          title={brand.data?.name ?? translate("backoffice.brands.title")}
          caption={
            brand.data === undefined
              ? undefined
              : translate("backoffice.brand.createdOn", {
                  date: formatDateTimeAsDate(brand.data.createdAt),
                })
          }
          navigation="back"
          onNavigate={() => router.replace(backofficeRoutes.brands)}
        />
      }
    >
      {brand.isPending ? <SkeletonDetail /> : null}
      {isBackofficeNotFound(brand.error) ? (
        <Banner message={translate("backoffice.brand.notFound")} />
      ) : brand.isError ? (
        <Banner message={translate("common.unexpectedError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => brand.refetch()}
          />
        </Banner>
      ) : null}
      {brand.data === undefined ? null : <BrandSections brand={brand.data} canManage={canManage} />}
    </ScrollScreen>
  );
}
