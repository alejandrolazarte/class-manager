const defaultApiBaseUrl = "http://localhost:5000";
const enabledFlag = "true";

export const appConfiguration = {
  get apiBaseUrl(): string {
    return process.env.EXPO_PUBLIC_API_BASE_URL ?? defaultApiBaseUrl;
  },
  get plansContactUrl(): string | null {
    return process.env.EXPO_PUBLIC_PLANS_CONTACT_URL || null;
  },
  get errorReportingDsn(): string | null {
    return process.env.EXPO_PUBLIC_SENTRY_DSN || null;
  },
  get errorReportingRelease(): string | undefined {
    return process.env.EXPO_PUBLIC_SENTRY_RELEASE || undefined;
  },
  get areAndroidPasskeysEnabled(): boolean {
    return process.env.EXPO_PUBLIC_ANDROID_PASSKEYS === enabledFlag;
  },
};
