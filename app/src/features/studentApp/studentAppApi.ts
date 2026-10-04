import { httpClient } from "@/api/httpClient";
import { BrowserPushSubscription } from "@/features/studentApp/push/browserPush";
import {
  StudentAppHome,
  StudentAppInvitation,
  StudentAppMakeups,
  StudentAppPackClasses,
  StudentAppPushKey,
  StudentAppNews,
  StudentAppDelivery,
  StudentAppOrder,
  StudentAppShop,
  InviteStudentAppRequest,
  PlaceStudentAppOrderLine,
} from "@/features/studentApp/types";

const studentAppPath = "/api/student-app";
const clientsPath = "/api/clients";
const appInvitationSegment = "app-invitation";
const shopPath = `${studentAppPath}/shop`;
const ordersPath = `${studentAppPath}/orders`;
const cancellationSegment = "cancellation";
const newsPath = `${studentAppPath}/news`;
const seenSegment = "seen";

export function getStudentAppHome(): Promise<StudentAppHome> {
  return httpClient.get<StudentAppHome>(studentAppPath);
}

export function inviteStudentApp(
  clientId: string,
  request: InviteStudentAppRequest,
): Promise<StudentAppInvitation> {
  return httpClient.post<StudentAppInvitation>(
    `${clientsPath}/${encodeURIComponent(clientId)}/${appInvitationSegment}`,
    request,
  );
}

export function getStudentAppShop(): Promise<StudentAppShop> {
  return httpClient.get<StudentAppShop>(shopPath);
}

export function listStudentAppOrders(): Promise<StudentAppOrder[]> {
  return httpClient.get<StudentAppOrder[]>(ordersPath);
}

export function placeStudentAppOrder(
  lines: PlaceStudentAppOrderLine[],
  delivery: StudentAppDelivery | null,
): Promise<StudentAppOrder> {
  return httpClient.post<StudentAppOrder>(ordersPath, { lines, ...delivery });
}

export function cancelStudentAppOrder(orderId: string): Promise<StudentAppOrder> {
  return httpClient.put<StudentAppOrder>(
    `${ordersPath}/${encodeURIComponent(orderId)}/${cancellationSegment}`,
    {},
  );
}

export function getStudentAppNews(): Promise<StudentAppNews> {
  return httpClient.get<StudentAppNews>(newsPath);
}

export function markStudentAppNewsSeen(): Promise<void> {
  return httpClient.put<void>(`${newsPath}/${seenSegment}`, {});
}

const studentsSegment = "students";
const absencesSegment = "absences";

function absencePath(studentId: string, classGroupId: string, sessionDate: string): string {
  return [
    studentAppPath,
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
  return [studentAppPath, studentsSegment, encodeURIComponent(studentId), makeupsSegment].join("/");
}

function makeupPath(studentId: string, classGroupId: string, sessionDate: string): string {
  return [makeupsPath(studentId), encodeURIComponent(classGroupId), sessionDate].join("/");
}

export function getStudentAppMakeups(studentId: string): Promise<StudentAppMakeups> {
  return httpClient.get<StudentAppMakeups>(makeupsPath(studentId));
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
  return [studentAppPath, studentsSegment, encodeURIComponent(studentId), packClassesSegment].join(
    "/",
  );
}

function packClassPath(studentId: string, classGroupId: string, sessionDate: string): string {
  return [packClassesPath(studentId), encodeURIComponent(classGroupId), sessionDate].join("/");
}

export function getStudentAppPackClasses(studentId: string): Promise<StudentAppPackClasses> {
  return httpClient.get<StudentAppPackClasses>(packClassesPath(studentId));
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

const pushKeyPath = `${studentAppPath}/push-key`;
const pushSubscriptionPath = `${studentAppPath}/push-subscription`;

export function getPushKey(): Promise<StudentAppPushKey> {
  return httpClient.get<StudentAppPushKey>(pushKeyPath);
}

export function savePushSubscription(subscription: BrowserPushSubscription): Promise<void> {
  return httpClient.put<void>(pushSubscriptionPath, subscription);
}

export function removePushSubscription(endpoint: string): Promise<void> {
  return httpClient.delete<void>(
    `${pushSubscriptionPath}?endpoint=${encodeURIComponent(endpoint)}`,
  );
}
