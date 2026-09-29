import { httpClient } from "@/api/httpClient";
import {
  FamilyHome,
  FamilyInvitation,
  FamilyOrder,
  FamilyShop,
  InviteFamilyRequest,
  PlaceFamilyOrderLine,
} from "@/features/family/types";

const familyPath = "/api/family";
const clientsPath = "/api/clients";
const appInvitationSegment = "app-invitation";
const shopPath = `${familyPath}/shop`;
const ordersPath = `${familyPath}/orders`;
const cancellationSegment = "cancellation";

export function getFamilyHome(): Promise<FamilyHome> {
  return httpClient.get<FamilyHome>(familyPath);
}

export function inviteFamily(
  clientId: string,
  request: InviteFamilyRequest,
): Promise<FamilyInvitation> {
  return httpClient.post<FamilyInvitation>(
    `${clientsPath}/${encodeURIComponent(clientId)}/${appInvitationSegment}`,
    request,
  );
}

export function getFamilyShop(): Promise<FamilyShop> {
  return httpClient.get<FamilyShop>(shopPath);
}

export function listFamilyOrders(): Promise<FamilyOrder[]> {
  return httpClient.get<FamilyOrder[]>(ordersPath);
}

export function placeFamilyOrder(lines: PlaceFamilyOrderLine[]): Promise<FamilyOrder> {
  return httpClient.post<FamilyOrder>(ordersPath, { lines });
}

export function cancelFamilyOrder(orderId: string): Promise<FamilyOrder> {
  return httpClient.put<FamilyOrder>(
    `${ordersPath}/${encodeURIComponent(orderId)}/${cancellationSegment}`,
    {},
  );
}
