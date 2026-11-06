import { EmailUnsubscription } from "@/features/notifications/emailUnsubscribeApi";

export const unsubscribeToken = "unsubscribe-token";

export const subscribedToEmails: EmailUnsubscription = {
  businessName: "Escuela Brazada",
  category: "Orders",
  isUnsubscribed: false,
};

export const unsubscribedFromEmails: EmailUnsubscription = {
  ...subscribedToEmails,
  isUnsubscribed: true,
};
