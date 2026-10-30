import { httpClient } from "@/api/httpClient";
import { BillingRedirect, OrganizationSubscription, Plan } from "@/features/subscriptions/types";

const plansPath = "/api/plans";
const organizationSubscriptionPath = "/api/organization/subscription";
const checkoutPath = `${organizationSubscriptionPath}/checkout`;
const portalPath = `${organizationSubscriptionPath}/portal`;

export function listPlans(): Promise<Plan[]> {
  return httpClient.get<Plan[]>(plansPath);
}

export function getOrganizationSubscription(): Promise<OrganizationSubscription> {
  return httpClient.get<OrganizationSubscription>(organizationSubscriptionPath);
}

export function startPlanCheckout(planCode: string): Promise<BillingRedirect> {
  return httpClient.post<BillingRedirect>(checkoutPath, { planCode });
}

export function openBillingPortal(): Promise<BillingRedirect> {
  return httpClient.post<BillingRedirect>(portalPath, {});
}
