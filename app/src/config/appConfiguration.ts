const defaultApiBaseUrl = "http://localhost:5000";

export const appConfiguration = {
  get apiBaseUrl(): string {
    return process.env.EXPO_PUBLIC_API_BASE_URL ?? defaultApiBaseUrl;
  },
  get plansContactUrl(): string | null {
    return process.env.EXPO_PUBLIC_PLANS_CONTACT_URL || null;
  },
};
