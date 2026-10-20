import { View } from "react-native";
import { planLabel, statusLabel, statusTone } from "@/backoffice/brands/brandLabels";
import { BrandSummary } from "@/backoffice/brands/brandsApi";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";
import { StatusPill } from "@/ui/StatusPill";

interface BrandListItemProps {
  brand: BrandSummary;
  onPress: (brand: BrandSummary) => void;
}

export function BrandListItem({ brand, onPress }: BrandListItemProps) {
  const owner = [brand.ownerName, brand.ownerEmail].filter(Boolean).join(" · ");
  return (
    <HairlineRow onPress={() => onPress(brand)} accessibilityLabel={brand.name}>
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong" numberOfLines={1}>
          {brand.name}
        </AppText>
        {owner ? (
          <AppText variant="caption" tone="muted" numberOfLines={1}>
            {owner}
          </AppText>
        ) : null}
        <AppText variant="footnote" tone="subtle" numberOfLines={1}>
          {translate("backoffice.brands.usage", {
            plan: planLabel(brand.planCode),
            branches: brand.branchCount,
            students: brand.studentCount,
          })}
        </AppText>
      </View>
      <StatusPill
        label={statusLabel(brand.status, brand.trialDaysLeft)}
        tone={statusTone(brand.status)}
        isSmall
      />
    </HairlineRow>
  );
}
