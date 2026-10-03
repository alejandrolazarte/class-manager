import { httpClient } from "@/api/httpClient";
import { OrganizationSubscription, Plan } from "@/features/subscriptions/types";

const plansPath = "/api/plans";
const organizationSubscriptionPath = "/api/organization/subscription";

export function listPlans(): Promise<Plan[]> {
  return httpClient.get<Plan[]>(plansPath);
}

export function getOrganizationSubscription(): Promise<OrganizationSubscription> {
  return httpClient.get<OrganizationSubscription>(organizationSubscriptionPath);
}
