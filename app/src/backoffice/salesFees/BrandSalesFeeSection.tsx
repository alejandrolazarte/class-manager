import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { brandSalesFeeQueryKey } from "@/backoffice/salesFees/salesFeeQueryKeys";
import { getBrandSalesFee, removeBrandSalesFee } from "@/backoffice/salesFees/salesFeesApi";
import { formatPercent } from "@/formatting/percent";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
import { HairlineSection } from "@/ui/HairlineSection";
import { useToast } from "@/ui/ToastProvider";

interface BrandSalesFeeSectionProps {
  brandId: string;
  canManage: boolean;
}

export function BrandSalesFeeSection({ brandId, canManage }: BrandSalesFeeSectionProps) {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const [isConfirmingRemoval, setIsConfirmingRemoval] = useState(false);
  const salesFee = useQuery({
    queryKey: brandSalesFeeQueryKey(brandId),
    queryFn: () => getBrandSalesFee(brandId),
    retry: false,
  });
  const remove = useMutation({
    mutationFn: () => removeBrandSalesFee(brandId),
    onSuccess: (updatedSalesFee) => {
      queryClient.setQueryData(brandSalesFeeQueryKey(brandId), updatedSalesFee);
      showToast(translate("backoffice.salesFee.removed"));
    },
    onError: () => showToast(translate("common.unexpectedError")),
    onSettled: () => setIsConfirmingRemoval(false),
  });
  const fee = salesFee.data;

  return (
    <HairlineSection title={translate("backoffice.salesFee.title")} isCompact>
      {salesFee.isError ? (
        <AppText tone="muted">{translate("common.unexpectedError")}</AppText>
      ) : null}
      {fee === undefined ? null : (
        <View className="gap-0.5">
          <AppText variant="bodyStrong">{formatPercent(fee.percent)}</AppText>
          <AppText variant="caption" tone="muted">
            {fee.isBrandFee
              ? translate("backoffice.salesFee.brandFee", {
                  planPercent: formatPercent(fee.planPercent),
                })
              : translate("backoffice.salesFee.planFee")}
          </AppText>
          {fee.note === null ? null : (
            <AppText variant="caption" tone="subtle">
              {fee.note}
            </AppText>
          )}
        </View>
      )}
      {canManage && fee?.isBrandFee === true ? (
        <Button
          variant="dangerGhost"
          size="medium"
          label={translate("backoffice.salesFee.backToPlan")}
          onPress={() => setIsConfirmingRemoval(true)}
        />
      ) : null}
      {canManage && fee !== undefined ? (
        <Button
          variant="dashed"
          label={translate("backoffice.salesFee.give")}
          onPress={() => router.push(backofficeRoutes.brandSalesFee(brandId))}
        />
      ) : null}
      {isConfirmingRemoval && fee !== undefined ? (
        <ConfirmDialog
          title={translate("backoffice.salesFee.backToPlanQuestion")}
          message={translate("backoffice.salesFee.backToPlanMessage", {
            planPercent: formatPercent(fee.planPercent),
          })}
          confirmLabel={translate("backoffice.salesFee.backToPlanConfirm")}
          onCancel={() => setIsConfirmingRemoval(false)}
          onConfirm={() => remove.mutate()}
          isConfirming={remove.isPending}
        />
      ) : null}
    </HairlineSection>
  );
}
