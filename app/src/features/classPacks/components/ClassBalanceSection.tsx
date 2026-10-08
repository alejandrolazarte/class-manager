import { useRouter } from "expo-router";
import { useState } from "react";
import { Pressable, View } from "react-native";
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
import { DeleteConfirmation } from "@/ui/DeleteConfirmation";
import { Icon } from "@/ui/Icon";
import { StatusPill } from "@/ui/StatusPill";
import { useToast } from "@/ui/ToastProvider";
import { useCanUndoCollection } from "@/features/fees/useCanUndoCollection";

interface ClassBalanceSectionProps {
  clientId: string;
  isMonthlyPlan?: boolean;
}

interface PurchaseGroup {
  key: string;
  name: string;
  classCount: number;
  purchases: ClassPackUsage[];
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

function groupPurchases(purchases: readonly ClassPackUsage[]): PurchaseGroup[] {
  const groups = new Map<string, PurchaseGroup>();
  for (const purchase of purchases) {
    const isFinished = purchase.status !== "Active";
    const key = `${purchase.name}|${purchase.classCount}|${isFinished}`;
    const group = groups.get(key) ?? {
      key,
      name: purchase.name,
      classCount: purchase.classCount,
      purchases: [],
    };
    group.purchases.push(purchase);
    groups.set(key, group);
  }
  return [...groups.values()];
}

interface PurchaseRowProps {
  usage: ClassPackUsage;
  title: string;
  details: (string | null)[];
  canUndo: boolean;
  onUndo: () => void;
}

function PurchaseRow({ usage, title, details, canUndo, onUndo }: PurchaseRowProps) {
  return (
    <View
      className={`flex-row items-center gap-3 ${usage.status === "Active" ? "" : "opacity-60"}`}
    >
      <View className="min-w-0 flex-1">
        <AppText variant="body">{title}</AppText>
        <AppText variant="caption" tone="muted">
          {details.filter(Boolean).join(" · ")}
        </AppText>
      </View>
      {canUndo ? (
        <Pressable
          accessibilityRole="button"
          accessibilityLabel={`${translate("classPacks.balance.deletePurchase")} ${usage.name}`}
          onPress={onUndo}
          className="py-2"
        >
          <AppText variant="label" tone="danger">
            {translate("classPacks.balance.deletePurchase")}
          </AppText>
        </Pressable>
      ) : null}
    </View>
  );
}

export function ClassBalanceSection({ clientId, isMonthlyPlan = false }: ClassBalanceSectionProps) {
  const canSellClassPacks = useCan(permissions.classPacksSell);
  const canUndoSale = useCanUndoCollection(permissions.classPacksSell);
  const router = useRouter();
  const clientTab = useCurrentTab(clientTabs);
  const currencyCode = useBusinessCurrency();
  const { data: balance, isPending } = useClassBalance(clientId);
  const deletePurchaseMutation = useDeleteClassPackPurchase();
  const { showToast } = useToast();
  const [purchasePendingDeletion, setPurchasePendingDeletion] = useState<ClassPackUsage | null>(
    null,
  );

  const deletePendingPurchase = async (purchaseId: string) => {
    try {
      await deletePurchaseMutation.mutateAsync(purchaseId);
      showToast(translate("classPacks.balance.purchaseDeleted"));
    } catch {
      showToast(translate("common.unexpectedError"));
    }
    setPurchasePendingDeletion(null);
  };
  const [openGroupKeys, setOpenGroupKeys] = useState<string[]>([]);

  if (isPending || balance === undefined) {
    return <Spinner className="mt-2" />;
  }

  const groups = groupPurchases(balance.purchases);
  const hasHistory = groups.length > 0 || balance.unpaidClasses > 0;
  const keepsClasses = isMonthlyPlan && balance.availableClasses > 0;
  const availableTone: TextTone = balance.availableClasses > 0 ? "success" : "muted";
  const toggleGroup = (groupKey: string) =>
    setOpenGroupKeys(
      openGroupKeys.includes(groupKey)
        ? openGroupKeys.filter((openKey) => openKey !== groupKey)
        : [...openGroupKeys, groupKey],
    );
  const purchaseDetails = (usage: ClassPackUsage) => [
    translate("classPacks.balance.usage", { used: usage.usedClasses, total: usage.classCount }),
    usage.classDurationMinutes
      ? translate("classPacks.list.duration", { minutes: usage.classDurationMinutes })
      : null,
    usageStatusLabel(usage),
  ];

  return (
    <Card className="gap-2.5 p-4">
      <View className="flex-row items-start justify-between gap-2">
        <View className="min-w-0 flex-1 gap-0.5">
          <AppText variant="overline" tone="subtle">
            {translate("classPacks.balance.purchasedTitle")}
          </AppText>
          {hasHistory || !isMonthlyPlan ? (
            <AppText variant="headline" tone={availableTone}>
              {translateCount("classPacks.balance.available", balance.availableClasses)}
            </AppText>
          ) : null}
        </View>
        {keepsClasses ? (
          <StatusPill label={translate("classPacks.balance.keep")} tone="success" isSmall />
        ) : null}
      </View>
      {keepsClasses ? (
        <AppText variant="caption" tone="muted">
          {translate("classPacks.balance.keepHint")}
        </AppText>
      ) : null}
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
      {groups.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate("classPacks.balance.noPurchases")}
        </AppText>
      ) : null}
      {groups.map((group) => {
        if (group.purchases.length === 1) {
          const usage = group.purchases[0];
          return (
            <View key={group.key} className="border-t border-border pt-2.5">
              <PurchaseRow
                usage={usage}
                title={`${usage.name} · ${formatMoney(usage.price, currencyCode)}`}
                details={purchaseDetails(usage)}
                canUndo={canUndoSale(usage.recordedByUserId)}
                onUndo={() => setPurchasePendingDeletion(usage)}
              />
            </View>
          );
        }
        const isOpen = openGroupKeys.includes(group.key);
        const usedClasses = group.purchases.reduce((total, usage) => total + usage.usedClasses, 0);
        const groupName = translate("classPacks.balance.groupTitle", {
          name: group.name,
          count: group.purchases.length,
        });
        return (
          <View key={group.key} className="gap-2.5 rounded-2xl bg-muted p-3">
            <Pressable
              accessibilityRole="button"
              accessibilityLabel={translate(
                isOpen ? "classPacks.balance.hidePacks" : "classPacks.balance.showPacks",
                { name: groupName },
              )}
              accessibilityState={{ expanded: isOpen }}
              onPress={() => toggleGroup(group.key)}
              className="flex-row items-center gap-2.5"
            >
              <Icon name="classPacks" size="medium" tone="primary" />
              <View className="min-w-0 flex-1">
                <AppText variant="bodyStrong">{groupName}</AppText>
                <AppText variant="caption" tone="muted">
                  {translate("classPacks.balance.groupUsage", {
                    used: usedClasses,
                    total: group.classCount * group.purchases.length,
                    classCount: group.classCount,
                  })}
                </AppText>
              </View>
              <Icon name={isOpen ? "collapse" : "expand"} tone="muted-foreground" />
            </Pressable>
            {isOpen
              ? group.purchases.map((usage, index) => (
                  <View key={usage.id} className="border-t border-border pt-2.5">
                    <PurchaseRow
                      usage={usage}
                      title={translate("classPacks.balance.packNumber", {
                        number: index + 1,
                        price: formatMoney(usage.price, currencyCode),
                      })}
                      details={[
                        ...purchaseDetails(usage),
                        translate("classPacks.balance.purchasedOn", {
                          date: formatBirthDateForDisplay(usage.purchasedOn),
                        }),
                      ]}
                      canUndo={canUndoSale(usage.recordedByUserId)}
                      onUndo={() => setPurchasePendingDeletion(usage)}
                    />
                  </View>
                ))
              : null}
          </View>
        );
      })}
      {purchasePendingDeletion ? (
        <DeleteConfirmation
          question={translate("classPacks.balance.deletePurchaseQuestion", {
            name: purchasePendingDeletion.name,
          })}
          confirmLabel={translate("classPacks.balance.confirmDeletePurchase")}
          onCancel={() => setPurchasePendingDeletion(null)}
          onConfirm={() => deletePendingPurchase(purchasePendingDeletion.id)}
          isDeleting={deletePurchaseMutation.isPending}
        />
      ) : null}
      {canSellClassPacks ? (
        <Button
          variant={isMonthlyPlan ? "secondary" : "primary"}
          size="medium"
          icon="classPacks"
          label={translate("classPacks.balance.sell")}
          onPress={() => router.push(routes.sellClassPack(clientTab, clientId))}
        />
      ) : null}
    </Card>
  );
}
