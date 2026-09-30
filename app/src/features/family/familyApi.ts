import { httpClient } from "@/api/httpClient";
import {
  FamilyHome,
  FamilyInvitation,
  FamilyNews,
  FamilyDelivery,
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
const newsPath = `${familyPath}/news`;
const seenSegment = "seen";

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

export function placeFamilyOrder(
  lines: PlaceFamilyOrderLine[],
  delivery: FamilyDelivery | null,
): Promise<FamilyOrder> {
  return httpClient.post<FamilyOrder>(ordersPath, { lines, ...delivery });
}

export function cancelFamilyOrder(orderId: string): Promise<FamilyOrder> {
  return httpClient.put<FamilyOrder>(
    `${ordersPath}/${encodeURIComponent(orderId)}/${cancellationSegment}`,
    {},
  );
}

export function getFamilyNews(): Promise<FamilyNews> {
  return httpClient.get<FamilyNews>(newsPath);
}

export function markFamilyNewsSeen(): Promise<void> {
  return httpClient.put<void>(`${newsPath}/${seenSegment}`, {});
}

const studentsSegment = "students";
const absencesSegment = "absences";

function absencePath(studentId: string, classGroupId: string, sessionDate: string): string {
  return [
    familyPath,
    studentsSegment,
    encodeURIComponent(studentId),
    absencesSegment,
    encodeURIComponent(classGroupId),
    sessionDate,
  ].join("/");
}

export function notifyAbsence(
  studentId: string,
  classGroupId: string,
  sessionDate: string,
): Promise<void> {
  return httpClient.put<void>(absencePath(studentId, classGroupId, sessionDate), {});
}

export function withdrawAbsence(
  studentId: string,
  classGroupId: string,
  sessionDate: string,
): Promise<void> {
  return httpClient.delete<void>(absencePath(studentId, classGroupId, sessionDate));
}
