import { useRouter } from "expo-router";
import { Pressable, View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { ClassPackUsage } from "@/features/classPacks/types";
import { useDeleteClassPackPurchase } from "@/features/classPacks/useClassPackMutations";
import { useClassBalance } from "@/features/classPacks/useClassPacks";
import { formatMoney } from "@/features/fees/money";
import { formatBirthDateForDisplay } from "@/features/students/birthDateFormatting";
import { translate, translateCount } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText, TextTone } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Spinner } from "@/ui/Spinner";

interface ClassBalanceSectionProps {
  clientId: string;
}

function usageStatusLabel(usage: ClassPackUsage): string {
  if (usage.status === "Expired") {
    return translate("classPacks.balance.expired");
  }
  if (usage.status === "UsedUp") {
    return translate("classPacks.balance.usedUp");
  }
  return usage.expiresOn === null
    ? translate("classPacks.list.noExpiry")
    : translate("classPacks.balance.expiresOn", {
        date: formatBirthDateForDisplay(usage.expiresOn),
      });
}

export function ClassBalanceSection({ clientId }: ClassBalanceSectionProps) {
  const router = useRouter();
  const currencyCode = useBusinessCurrency();
  const { data: balance, isPending } = useClassBalance(clientId);
  const deletePurchaseMutation = useDeleteClassPackPurchase();

  if (isPending || balance === undefined) {
    return <Spinner className="mt-2" />;
  }

  const availableTone: TextTone = balance.availableClasses > 0 ? "success" : "muted";
  return (
    <View className="gap-2 rounded-xl bg-surface p-3">
      <AppText variant="label" tone="subtle">
        {translate("classPacks.balance.title")}
      </AppText>
      <AppText variant="bodyStrong" tone={availableTone}>
        {translateCount("classPacks.balance.available", balance.availableClasses)}
      </AppText>
      {balance.unpaidClasses > 0 ? (
        <View className="gap-1">
          <AppText variant="bodyStrong" tone="danger">
            {translateCount("classPacks.balance.unpaid", balance.unpaidClasses)}
          </AppText>
          {balance.unpaidAttendances.map((attended, index) => (
            <AppText key={`${attended.date}-${index}`} variant="caption" tone="muted">
              {`${formatBirthDateForDisplay(attended.date)} · ${attended.studentFullName} · ${attended.classGroupName}`}
            </AppText>
          ))}
        </View>
      ) : null}
      {balance.purchases.map((usage) => (
        <View
          key={usage.id}
          className={`flex-row items-center gap-3 border-t border-border-subtle pt-2 ${usage.status === "Active" ? "" : "opacity-60"}`}
        >
          <View className="flex-1">
            <AppText variant="body">{`${usage.name} · ${formatMoney(usage.price, currencyCode)}`}</AppText>
            <AppText variant="caption" tone="muted">
              {`${translate("classPacks.balance.usage", { used: usage.usedClasses, total: usage.classCount })} · ${usageStatusLabel(usage)}`}
            </AppText>
          </View>
          <Pressable
            accessibilityRole="button"
            accessibilityLabel={`${translate("classPacks.balance.deletePurchase")} ${usage.name}`}
            onPress={() => deletePurchaseMutation.mutate(usage.id)}
          >
            <AppText variant="label" tone="danger">
              {translate("classPacks.balance.deletePurchase")}
            </AppText>
          </Pressable>
        </View>
      ))}
      <Button
        label={translate("classPacks.balance.sell")}
        onPress={() => router.push(routes.sellClassPack(clientId))}
      />
    </View>
  );
}
