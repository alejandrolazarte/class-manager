const defaultApiBaseUrl = "http://localhost:5000";

export const appConfiguration = {
  get apiBaseUrl(): string {
    return process.env.EXPO_PUBLIC_API_BASE_URL ?? defaultApiBaseUrl;
  },
  get supportWhatsAppNumber(): string | null {
    return process.env.EXPO_PUBLIC_SUPPORT_WHATSAPP_NUMBER || null;
  },
};
