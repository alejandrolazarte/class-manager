export const routes = {
  clients: "/clients",
  newClient: "/clients/new",
  clientDetail: (clientId: string) => `/clients/${clientId}`,
  signIn: "/sign-in",
  signUp: "/sign-up",
  settings: "/settings",
  businessSettings: "/settings/business",
} as const;
