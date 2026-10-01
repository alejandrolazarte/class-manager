import { useRouter } from "expo-router";
import { Linking, Pressable, View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { ClassPackUsage } from "@/features/classPacks/types";
import { useDeleteClassPackPurchase } from "@/features/classPacks/useClassPackMutations";
import { useClassBalance } from "@/features/classPacks/useClassPacks";
import { formatMoney } from "@/features/fees/money";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { formatBirthDateForDisplay } from "@/features/students/birthDateFormatting";
import { translate, translateCount } from "@/i18n/translate";
import { clientTabs, routes } from "@/navigation/routes";
import { useCurrentTab } from "@/navigation/useCurrentTab";
import { AppText, TextTone } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Spinner } from "@/ui/Spinner";
import { Card } from "@/ui/Card";
import { useCanUndoCollection } from "@/features/fees/useCanUndoCollection";

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
  const canSellClassPacks = useCan(permissions.classPacksSell);
  const canUndoSale = useCanUndoCollection(permissions.classPacksSell);
  const router = useRouter();
  const clientTab = useCurrentTab(clientTabs);
  const currencyCode = useBusinessCurrency();
  const { data: balance, isPending } = useClassBalance(clientId);
  const deletePurchaseMutation = useDeleteClassPackPurchase();

  if (isPending || balance === undefined) {
    return <Spinner className="mt-2" />;
  }

  const availableTone: TextTone = balance.availableClasses > 0 ? "success" : "muted";
  return (
    <Card className="gap-2.5 p-4">
      <AppText variant="overline" tone="subtle">
        {translate("classPacks.balance.title")}
      </AppText>
      <AppText variant="title" tone={availableTone}>
        {translateCount("classPacks.balance.available", balance.availableClasses)}
      </AppText>
      {balance.unpaidClasses > 0 ? (
        <View className="gap-1">
          <AppText variant="bodyStrong" tone="danger">
            {translateCount("classPacks.balance.unpaid", balance.unpaidClasses)}
          </AppText>
          {balance.unpaidAttendances.map((attended, index) => (
            <AppText key={`${attended.date}-${index}`} variant="caption" tone="muted">
              {`${formatBirthDateForDisplay(attended.date)} · ${attended.studentFullName} · ${attended.isPrivateLesson ? translate("privateLessons.withCoach", { coach: attended.classGroupName }) : attended.classGroupName}`}
            </AppText>
          ))}
        </View>
      ) : null}
      {balance.purchases.map((usage) => (
        <View
          key={usage.id}
          className={`flex-row items-center gap-3 border-t border-border pt-2.5 ${usage.status === "Active" ? "" : "opacity-60"}`}
        >
          <View className="flex-1">
            <AppText variant="body">{`${usage.name} · ${formatMoney(usage.price, currencyCode)}`}</AppText>
            <AppText variant="caption" tone="muted">
              {[
                translate("classPacks.balance.usage", {
                  used: usage.usedClasses,
                  total: usage.classCount,
                }),
                usage.classDurationMinutes
                  ? translate("classPacks.list.duration", { minutes: usage.classDurationMinutes })
                  : null,
                usageStatusLabel(usage),
              ]
                .filter(Boolean)
                .join(" · ")}
            </AppText>
            {usage.materialUrl ? (
              <Pressable
                accessibilityRole="link"
                accessibilityLabel={translate("classPacks.balance.openMaterial", {
                  name: usage.name,
                })}
                onPress={() => Linking.openURL(usage.materialUrl ?? "")}
                className="self-start py-1"
              >
                <AppText variant="link" tone="primary">
                  {translate("classPacks.balance.material")}
                </AppText>
              </Pressable>
            ) : null}
          </View>
          {canUndoSale(usage.recordedByUserId) ? (
            <Pressable
              accessibilityRole="button"
              accessibilityLabel={`${translate("classPacks.balance.deletePurchase")} ${usage.name}`}
              onPress={() => deletePurchaseMutation.mutate(usage.id)}
            >
              <AppText variant="label" tone="danger">
                {translate("classPacks.balance.deletePurchase")}
              </AppText>
            </Pressable>
          ) : null}
        </View>
      ))}
      {canSellClassPacks ? (
        <Button
          size="medium"
          icon="classPacks"
          label={translate("classPacks.balance.sell")}
          onPress={() => router.push(routes.sellClassPack(clientTab, clientId))}
        />
      ) : null}
    </Card>
  );
}
