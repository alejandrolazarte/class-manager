import { View } from "react-native";
import {
  orderStage,
  orderStageLabels,
  orderStageTones,
  orderStepIndex,
  orderSteps,
} from "@/features/family/familyOrderStage";
import { FamilyOrder } from "@/features/family/types";
import { useCancelFamilyOrder } from "@/features/family/useFamilyShop";
import { formatMoney } from "@/features/fees/money";
import { formatLongDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { AppText, TextTone } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Icon, IconName } from "@/ui/Icon";
import { StatusPill } from "@/ui/StatusPill";
import { useToast } from "@/ui/ToastProvider";

const isoDateLength = 10;

function inlineDate(isoDateTime: string): string {
  const longDate = formatLongDate(isoDateTime.slice(0, isoDateLength));
  return longDate.charAt(0).toLocaleLowerCase() + longDate.slice(1);
}

type NoticeTone = "warning" | "success" | "neutral";

interface OrderNotice {
  message: string;
  icon: IconName;
  tone: NoticeTone;
}

const noticeClassNames: Record<NoticeTone, string> = {
  warning: "bg-warning-soft",
  success: "bg-success-soft",
  neutral: "bg-muted",
};

const noticeTextTones: Record<NoticeTone, TextTone> = {
  warning: "warningSoft",
  success: "successSoft",
  neutral: "muted",
};

const noticeIconTones = {
  warning: "warning-soft-foreground",
  success: "success-soft-foreground",
  neutral: "muted-foreground",
} as const;

function deliveryPlace(order: FamilyOrder): string {
  return order.delivery === "InClass" && order.deliveryClassGroupName !== null
    ? translate("family.orders.deliveryInClass", { className: order.deliveryClassGroupName })
    : translate("family.orders.deliveryPickup");
}

function noticeOf(order: FamilyOrder): OrderNotice | null {
  const stage = orderStage(order);
  if (stage === "awaitingPayment") {
    return {
      message: translate("family.orders.payBeforeOrCancelled", {
        date: inlineDate(order.expiresAt),
      }),
      icon: "schedule",
      tone: "warning",
    };
  }
  if (stage === "ready") {
    return {
      message: `${deliveryPlace(order)}.`,
      icon: order.delivery === "InClass" ? "students" : "business",
      tone: "success",
    };
  }
  if (stage === "preparing") {
    return {
      message: translate("family.orders.notifyWhenReady", { where: deliveryPlace(order) }),
      icon: "info",
      tone: "neutral",
    };
  }
  return null;
}

function OrderSteps({ currentStep }: { currentStep: number }) {
  return (
    <View className="flex-row gap-1">
      {orderSteps.map((stepKey, stepIndex) => {
        const isReached = stepIndex <= currentStep;
        return (
          <View key={stepKey} className="flex-1 gap-[5px]">
            <View className={`h-1.5 rounded-full ${isReached ? "bg-primary" : "bg-muted"}`} />
            <AppText
              variant="footnote"
              tone={isReached ? "default" : "disabled"}
              className="font-label"
            >
              {translate(stepKey)}
            </AppText>
          </View>
        );
      })}
    </View>
  );
}

export function FamilyOrderCard({
  order,
  currencyCode,
}: {
  order: FamilyOrder;
  currencyCode: string;
}) {
  const { showToast } = useToast();
  const cancelFamilyOrderMutation = useCancelFamilyOrder();
  const stage = orderStage(order);
  const currentStep = orderStepIndex(order);
  const notice = noticeOf(order);

  const cancel = async () => {
    try {
      await cancelFamilyOrderMutation.mutateAsync(order.id);
      showToast(translate("family.orders.cancelled"));
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  return (
    <Card className={`gap-3 px-4 py-3.5 ${stage === "cancelled" ? "opacity-65" : ""}`}>
      <View className="flex-row items-start justify-between gap-3">
        <View className="min-w-0 flex-1 gap-1">
          <StatusPill label={translate(orderStageLabels[stage])} tone={orderStageTones[stage]} />
          <AppText variant="caption" tone="subtle">
            {translate("family.orders.placedOn", {
              number: order.number,
              date: inlineDate(order.createdAt),
            })}
          </AppText>
        </View>
        <AppText variant="heading" className="font-heavy">
          {formatMoney(order.total, currencyCode)}
        </AppText>
      </View>
      {currentStep === null ? null : <OrderSteps currentStep={currentStep} />}
      <View className="gap-2">
        {order.lines.map((line) => (
          <View key={line.id} className="flex-row items-center gap-2.5">
            <View
              className={`h-9 w-9 items-center justify-center rounded-[10px] ${line.kind === "ClassPack" ? "bg-primary-soft" : "bg-muted"}`}
            >
              <Icon
                name={line.kind === "ClassPack" ? "classPacks" : "products"}
                size="medium"
                tone="primary-soft-foreground"
              />
            </View>
            <AppText variant="bodyStrong" tone="muted" className="flex-1 font-label">
              {line.quantity > 1 ? `${line.quantity} × ${line.name}` : line.name}
            </AppText>
          </View>
        ))}
      </View>
      {notice === null ? null : (
        <View
          className={`flex-row items-start gap-2 rounded-[14px] px-3 py-2.5 ${noticeClassNames[notice.tone]}`}
        >
          <Icon name={notice.icon} size="medium" tone={noticeIconTones[notice.tone]} />
          <AppText
            variant="label"
            tone={noticeTextTones[notice.tone]}
            className="flex-1 leading-[17px]"
          >
            {notice.message}
          </AppText>
        </View>
      )}
      {stage === "awaitingPayment" ? (
        <Button
          size="medium"
          variant="dangerOutline"
          label={translate("family.orders.cancel")}
          onPress={cancel}
          isLoading={cancelFamilyOrderMutation.isPending}
        />
      ) : null}
    </Card>
  );
}
