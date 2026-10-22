import { View } from "react-native";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import {
  isCountedFeature,
  lockedMessageKey,
  PlanOffer,
  usageOf,
} from "@/features/subscriptions/planOffer";
import { planCodes } from "@/features/subscriptions/subscriptionCodes";
import { featureDescription, featureLabel } from "@/features/subscriptions/subscriptionLabels";
import { useSubscription } from "@/features/subscriptions/useSubscription";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { BottomSheet } from "@/ui/BottomSheet";
import { Button } from "@/ui/Button";
import { IconCircle } from "@/ui/IconCircle";
import { ProgressBar } from "@/ui/ProgressBar";

interface PlanOfferSheetProps {
  offer: PlanOffer;
  onSeePlans: () => void;
  onClose: () => void;
}

interface OfferTexts {
  title?: string;
  message?: string;
}

function featureOfferTexts(featureCode: string): OfferTexts {
  const messageKey = lockedMessageKey(featureCode);
  const message = messageKey === undefined ? undefined : translate(messageKey);
  return isCountedFeature(featureCode)
    ? { title: message }
    : { title: featureLabel(featureCode), message };
}

export function PlanOfferSheet({ offer, onSeePlans, onClose }: PlanOfferSheetProps) {
  const subscription = useSubscription();
  const canChangePlan = useCan(permissions.subscriptionView);
  const isEnded = offer.kind === "ended";
  const usage = isEnded ? undefined : usageOf(subscription, offer.featureCode);
  const texts: OfferTexts = isEnded
    ? {
        title: translate(
          subscription.planCode === planCodes.free
            ? "subscriptions.ended.trialTitle"
            : "subscriptions.ended.title",
        ),
        message: translate("subscriptions.ended.body"),
      }
    : featureOfferTexts(offer.featureCode);
  const isLimit = usage !== undefined;

  return (
    <BottomSheet onClose={onClose}>
      <View className="items-center gap-3.5">
        <IconCircle
          icon={isEnded ? "plan" : "locked"}
          tone={isLimit ? "warning" : "primary"}
          size="large"
        />
        {usage ? (
          <View className="w-full gap-1.5">
            <View className="flex-row">
              <ProgressBar ratio={1} tone="warning" />
            </View>
            <AppText variant="caption" tone="muted" className="text-center">
              {featureDescription(usage)}
            </AppText>
          </View>
        ) : null}
        {texts.title ? (
          <AppText variant="headline" accessibilityRole="header" className="text-center">
            {texts.title}
          </AppText>
        ) : null}
        {texts.message ? (
          <AppText variant="body" tone="muted" className="text-center">
            {texts.message}
          </AppText>
        ) : null}
        {canChangePlan ? null : (
          <AppText variant="body" tone="muted" className="text-center">
            {translate("subscriptions.askOwner")}
          </AppText>
        )}
      </View>
      {canChangePlan ? (
        <Button label={translate("subscriptions.seePlans")} onPress={onSeePlans} isExpanded />
      ) : null}
      <Button variant="secondary" label={translate("common.back")} onPress={onClose} isExpanded />
    </BottomSheet>
  );
}
