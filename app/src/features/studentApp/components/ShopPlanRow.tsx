import { View } from "react-native";
import { CatalogThumbnail } from "@/features/catalogImages/components/CatalogThumbnail";
import { monthlyPlanHint } from "@/features/fees/planDescriptions";
import { StudentAppShopPlan } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";
import { Icon } from "@/ui/Icon";

interface ShopPlanRowProps {
  plan: StudentAppShopPlan;
  currencyCode: string;
  isFamilyPlan: boolean;
  onOpen: () => void;
}

export function ShopPlanRow({ plan, currencyCode, isFamilyPlan, onOpen }: ShopPlanRowProps) {
  return (
    <HairlineRow onPress={onOpen} accessibilityLabel={plan.name}>
      <CatalogThumbnail imageUrl={plan.images[0]?.url ?? null} placeholderIcon="classPacks" />
      <View className="flex-1 gap-1">
        <AppText variant="bodyStrong">{plan.name}</AppText>
        <AppText variant="caption" tone="subtle">
          {[
            monthlyPlanHint(plan, currencyCode),
            isFamilyPlan ? translate("student.shop.yourPlan") : null,
          ]
            .filter(Boolean)
            .join(" · ")}
        </AppText>
      </View>
      <Icon name="next" size="medium" tone="subtle-foreground" />
    </HairlineRow>
  );
}
