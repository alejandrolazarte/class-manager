export const routes = {
  students: "/students",
  registerClient: "/students/new",
  clientDetail: (clientId: string) => `/students/clients/${clientId}`,
  addStudent: (clientId: string) => `/students/clients/${clientId}/new-student`,
  signIn: "/sign-in",
  signUp: "/sign-up",
  settings: "/settings",
  businessSettings: "/settings/business",
} as const;
