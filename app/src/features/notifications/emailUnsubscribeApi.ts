import { anonymousHttpClient } from "@/api/httpClient";
import { NotificationCategory } from "@/features/notifications/notificationPreferenceTypes";

export interface EmailUnsubscription {
  businessName: string;
  category: NotificationCategory;
  isUnsubscribed: boolean;
}

const emailUnsubscribePath = "/api/email-unsubscribe";
const resubscribePath = "/api/email-unsubscribe/resubscribe";

function withToken(path: string, token: string): string {
  return `${path}?token=${encodeURIComponent(token)}`;
}

export function getEmailUnsubscription(token: string): Promise<EmailUnsubscription> {
  return anonymousHttpClient.get<EmailUnsubscription>(withToken(emailUnsubscribePath, token));
}

export function unsubscribeFromEmail(token: string): Promise<EmailUnsubscription> {
  return anonymousHttpClient.post<EmailUnsubscription>(withToken(emailUnsubscribePath, token), {});
}

export function resubscribeToEmail(token: string): Promise<EmailUnsubscription> {
  return anonymousHttpClient.post<EmailUnsubscription>(withToken(resubscribePath, token), {});
}
