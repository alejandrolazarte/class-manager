import { useQuery } from "@tanstack/react-query";
import { useCurrentMember } from "@/features/members/CurrentMemberProvider";
import { daysBetween } from "@/features/home/agenda";
import { todayIsoDate } from "@/features/sessions/dates";
import { FeatureCode, planCodes } from "@/features/subscriptions/subscriptionCodes";
import { subscriptionQueryKeys } from "@/features/subscriptions/subscriptionQueryKeys";
import { getOrganizationSubscription, listPlans } from "@/features/subscriptions/subscriptionsApi";
import { CurrentSubscription } from "@/features/subscriptions/types";

const unrestrictedSubscription: CurrentSubscription = {
  planCode: planCodes.enterprise,
  isActive: true,
  endsOn: null,
  features: [],
};

export function useSubscription(): CurrentSubscription {
  return useCurrentMember().subscription ?? unrestrictedSubscription;
}

export function useFeature(featureCode: FeatureCode): boolean {
  const { subscription } = useCurrentMember();
  return (
    subscription == null || subscription.features.some((feature) => feature.code === featureCode)
  );
}

export function useAllowsAnother(featureCode: FeatureCode, currentCount: number): boolean {
  const { subscription } = useCurrentMember();
  if (subscription == null) {
    return true;
  }
  const feature = subscription.features.find((candidate) => candidate.code === featureCode);
  return feature !== undefined && (feature.limit === null || currentCount < feature.limit);
}

export function useIsAtLimit(featureCode: FeatureCode): boolean {
  const { subscription } = useCurrentMember();
  const feature = subscription?.features.find((candidate) => candidate.code === featureCode);
  return (
    feature !== undefined &&
    feature.limit !== null &&
    feature.used != null &&
    feature.used >= feature.limit
  );
}

export function daysLeftOf(endsOn: string, today: string = todayIsoDate()): number {
  return daysBetween(today, endsOn) + 1;
}

export function isTrial(subscription: CurrentSubscription): boolean {
  return subscription.planCode === planCodes.free && subscription.endsOn !== null;
}

export function usePlans() {
  return useQuery({ queryKey: subscriptionQueryKeys.plans(), queryFn: listPlans });
}

export function useOrganizationSubscription({ enabled }: { enabled: boolean }) {
  return useQuery({
    queryKey: subscriptionQueryKeys.organizationSubscription(),
    queryFn: getOrganizationSubscription,
    enabled,
  });
}
