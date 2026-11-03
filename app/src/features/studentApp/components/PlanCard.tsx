import { ClassOfferCard } from "@/features/studentApp/components/ClassOfferCard";
import { StudentAppShopPlan } from "@/features/studentApp/types";
import { formatMoney } from "@/formatting/money";
import { translate, translateCount } from "@/i18n/translate";

interface PlanCardProps {
  plan: StudentAppShopPlan;
  currencyCode: string;
  isFamilyPlan: boolean;
  onOpen: () => void;
}

const detailSeparator = " · ";

export function planBadge(plan: StudentAppShopPlan): string {
  return plan.classCount === null
    ? translate("student.shop.libreClasses")
    : translateCount("student.shop.classesPerMonth", plan.classCount);
}

export function PlanCard({ plan, currencyCode, isFamilyPlan, onOpen }: PlanCardProps) {
  const actionLabel = translate("student.shop.seePlan");
  return (
    <ClassOfferCard
      name={plan.name}
      images={plan.images}
      badge={planBadge(plan)}
      details={[
        translate("student.shop.pricePerMonth", { price: formatMoney(plan.price, currencyCode) }),
        isFamilyPlan ? translate("student.shop.yourPlan") : null,
      ]
        .filter(Boolean)
        .join(detailSeparator)}
      onOpen={onOpen}
      action={{
        label: actionLabel,
        accessibilityLabel: `${actionLabel} ${plan.name}`,
        isSelected: isFamilyPlan,
        onPress: onOpen,
      }}
    />
  );
}
