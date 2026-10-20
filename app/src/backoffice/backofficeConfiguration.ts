const defaultBackofficeApiBaseUrl = "http://localhost:5100";

export const backofficeConfiguration = {
  get apiBaseUrl(): string {
    return process.env.EXPO_PUBLIC_API_BASE_URL ?? defaultBackofficeApiBaseUrl;
  },
};
