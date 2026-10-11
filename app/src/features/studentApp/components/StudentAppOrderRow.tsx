import { View } from "react-native";
import {
  orderStage,
  orderStageLabels,
  orderStageTones,
  orderStepIndex,
  orderSteps,
} from "@/features/studentApp/studentAppOrderStage";
import { StudentAppOrder } from "@/features/studentApp/types";
import { useCancelStudentAppOrder } from "@/features/studentApp/useStudentAppShop";
import { formatMoney } from "@/features/fees/money";
import { formatLongDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { HairlineRow } from "@/ui/HairlineRow";
import { HighlightTone } from "@/ui/Highlight";
import { Icon } from "@/ui/Icon";
import { Notice } from "@/ui/Notice";
import { ProgressSteps } from "@/ui/ProgressSteps";
import { StatusPill } from "@/ui/StatusPill";
import { useToast } from "@/ui/ToastProvider";

const isoDateLength = 10;

function inlineDate(isoDateTime: string): string {
  const longDate = formatLongDate(isoDateTime.slice(0, isoDateLength));
  return longDate.charAt(0).toLocaleLowerCase() + longDate.slice(1);
}

interface OrderNotice {
  message: string;
  tone: HighlightTone;
}

function deliveryPlace(order: StudentAppOrder): string {
  return order.delivery === "InClass" && order.deliveryClassGroupName !== null
    ? translate("student.orders.deliveryInClass", { className: order.deliveryClassGroupName })
    : translate("student.orders.deliveryPickup");
}

function noticeOf(order: StudentAppOrder): OrderNotice | null {
  const stage = orderStage(order);
  if (stage === "awaitingPayment") {
    return {
      message: translate("student.orders.payBeforeOrCancelled", {
        date: inlineDate(order.expiresAt),
      }),
      tone: "warning",
    };
  }
  if (stage === "ready") {
    return {
      message: `${deliveryPlace(order)}.`,
      tone: "success",
    };
  }
  if (stage === "preparing") {
    return {
      message: translate("student.orders.notifyWhenReady", { where: deliveryPlace(order) }),
      tone: "primary",
    };
  }
  return null;
}

export function StudentAppOrderRow({ order }: { order: StudentAppOrder }) {
  const { showToast } = useToast();
  const cancelStudentAppOrderMutation = useCancelStudentAppOrder();
  const stage = orderStage(order);
  const currentStep = orderStepIndex(order);
  const notice = noticeOf(order);

  const cancel = async () => {
    try {
      await cancelStudentAppOrderMutation.mutateAsync(order.id);
      showToast(translate("student.orders.cancelled"));
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  return (
    <HairlineRow isDimmed={stage === "cancelled"} className="gap-3 py-3.5">
      <View className="flex-row items-start justify-between gap-3">
        <View className="min-w-0 flex-1 gap-1">
          <StatusPill
            isSmall
            label={translate(orderStageLabels[stage])}
            tone={orderStageTones[stage]}
          />
          <AppText variant="footnote" tone="muted">
            {translate("student.orders.placedOn", {
              number: order.number,
              date: inlineDate(order.createdAt),
            })}
          </AppText>
        </View>
        <AppText variant="heading">{formatMoney(order.total, order.currencyCode)}</AppText>
      </View>
      {currentStep === null ? null : (
        <ProgressSteps
          steps={orderSteps.map((stepKey) => translate(stepKey))}
          currentStep={currentStep}
        />
      )}
      <View className="gap-1.5">
        {order.lines.map((line) => (
          <View key={line.id} className="flex-row items-center gap-2.5">
            <View
              className={`h-[30px] w-[30px] items-center justify-center rounded-lg ${line.kind === "ClassPack" ? "bg-primary-soft" : "bg-muted"}`}
            >
              <Icon
                name={line.kind === "ClassPack" ? "classPacks" : "products"}
                size="small"
                tone={line.kind === "ClassPack" ? "primary-soft-foreground" : "muted-foreground"}
              />
            </View>
            <AppText variant="body" tone="muted" className="flex-1">
              {line.quantity > 1 ? `${line.quantity} × ${line.name}` : line.name}
            </AppText>
          </View>
        ))}
      </View>
      {notice === null ? null : <Notice tone={notice.tone} message={notice.message} />}
      {stage === "awaitingPayment" ? (
        <Button
          size="medium"
          variant="dangerOutline"
          label={translate("student.orders.cancel")}
          onPress={cancel}
          isLoading={cancelStudentAppOrderMutation.isPending}
        />
      ) : null}
    </HairlineRow>
  );
}
