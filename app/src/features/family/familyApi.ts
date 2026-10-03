import { httpClient } from "@/api/httpClient";
import { BrowserPushSubscription } from "@/features/family/push/browserPush";
import {
  FamilyHome,
  FamilyInvitation,
  FamilyMakeups,
  FamilyPackClasses,
  FamilyPushKey,
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

const makeupsSegment = "makeups";

function makeupsPath(studentId: string): string {
  return [familyPath, studentsSegment, encodeURIComponent(studentId), makeupsSegment].join("/");
}

function makeupPath(studentId: string, classGroupId: string, sessionDate: string): string {
  return [makeupsPath(studentId), encodeURIComponent(classGroupId), sessionDate].join("/");
}

export function getFamilyMakeups(studentId: string): Promise<FamilyMakeups> {
  return httpClient.get<FamilyMakeups>(makeupsPath(studentId));
}

export function bookMakeup(
  studentId: string,
  classGroupId: string,
  sessionDate: string,
): Promise<void> {
  return httpClient.put<void>(makeupPath(studentId, classGroupId, sessionDate), {});
}

export function cancelMakeup(
  studentId: string,
  classGroupId: string,
  sessionDate: string,
): Promise<void> {
  return httpClient.delete<void>(makeupPath(studentId, classGroupId, sessionDate));
}

const packClassesSegment = "pack-classes";

function packClassesPath(studentId: string): string {
  return [familyPath, studentsSegment, encodeURIComponent(studentId), packClassesSegment].join("/");
}

function packClassPath(studentId: string, classGroupId: string, sessionDate: string): string {
  return [packClassesPath(studentId), encodeURIComponent(classGroupId), sessionDate].join("/");
}

export function getFamilyPackClasses(studentId: string): Promise<FamilyPackClasses> {
  return httpClient.get<FamilyPackClasses>(packClassesPath(studentId));
}

export function bookPackClass(
  studentId: string,
  classGroupId: string,
  sessionDate: string,
): Promise<void> {
  return httpClient.put<void>(packClassPath(studentId, classGroupId, sessionDate), {});
}

export function cancelPackClass(
  studentId: string,
  classGroupId: string,
  sessionDate: string,
): Promise<void> {
  return httpClient.delete<void>(packClassPath(studentId, classGroupId, sessionDate));
}

const pushKeyPath = `${familyPath}/push-key`;
const pushSubscriptionPath = `${familyPath}/push-subscription`;

export function getPushKey(): Promise<FamilyPushKey> {
  return httpClient.get<FamilyPushKey>(pushKeyPath);
}

export function savePushSubscription(subscription: BrowserPushSubscription): Promise<void> {
  return httpClient.put<void>(pushSubscriptionPath, subscription);
}

export function removePushSubscription(endpoint: string): Promise<void> {
  return httpClient.delete<void>(
    `${pushSubscriptionPath}?endpoint=${encodeURIComponent(endpoint)}`,
  );
}
