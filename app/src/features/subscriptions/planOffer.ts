import { isApiError } from "@/api/apiErrors";
import { getStringExtension } from "@/api/problemDetails";
import {
  countedFeatureCodes,
  FeatureCode,
  featureCodes,
  subscriptionErrorCodes,
} from "@/features/subscriptions/subscriptionCodes";
import { CurrentSubscription, FeatureLimit } from "@/features/subscriptions/types";
import { TranslationKey } from "@/i18n/translate";

const featureExtensionName = "feature";

export type PlanOffer = { kind: "feature"; featureCode: string } | { kind: "ended" };

export const endedPlanOffer: PlanOffer = { kind: "ended" };

export function featurePlanOffer(featureCode: string): PlanOffer {
  return { kind: "feature", featureCode };
}

const lockedMessageKeys: Readonly<Record<FeatureCode, TranslationKey>> = {
  [featureCodes.students]: "subscriptions.locked.students",
  [featureCodes.branches]: "subscriptions.locked.branches",
  [featureCodes.team]: "subscriptions.locked.team",
  [featureCodes.classPacks]: "subscriptions.locked.classPacks",
  [featureCodes.importExport]: "subscriptions.locked.importExport",
  [featureCodes.customRoles]: "subscriptions.locked.customRoles",
  [featureCodes.studentApp]: "subscriptions.locked.studentApp",
  [featureCodes.shop]: "subscriptions.locked.shop",
  [featureCodes.brand]: "subscriptions.locked.brand",
  [featureCodes.documentStorage]: "subscriptions.locked.documentStorage",
};

export function lockedMessageKey(featureCode: string): TranslationKey | undefined {
  return (lockedMessageKeys as Readonly<Record<string, TranslationKey>>)[featureCode];
}

export function isCountedFeature(featureCode: string): boolean {
  return countedFeatureCodes.includes(featureCode);
}

export function planOfferOfRefusal(error: unknown): PlanOffer | null {
  if (!isApiError(error)) {
    return null;
  }
  const { code } = error.problem;
  const feature = getStringExtension(error.problem, featureExtensionName);
  if (code === subscriptionErrorCodes.inactive) {
    return endedPlanOffer;
  }
  const isFeatureRefusal =
    code === subscriptionErrorCodes.featureNotInPlan ||
    code === subscriptionErrorCodes.featureLimitReached;
  return isFeatureRefusal && feature !== undefined ? featurePlanOffer(feature) : null;
}

export function usageOf(
  subscription: CurrentSubscription | null | undefined,
  featureCode: string,
): FeatureLimit | undefined {
  const feature = subscription?.features.find((candidate) => candidate.code === featureCode);
  return feature !== undefined && feature.limit !== null && feature.used != null
    ? feature
    : undefined;
}
