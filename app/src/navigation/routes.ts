export const clientTabs = ["students", "fees"] as const;
export type ClientTab = (typeof clientTabs)[number];

export const orderTabs = ["settings", "today", "fees"] as const;
export type OrderTab = (typeof orderTabs)[number];

export const planTabs = ["settings", "today", "students"] as const;
export type PlanTab = (typeof planTabs)[number];

export const collectionsViews = ["fees", "orders"] as const;
export type CollectionsView = (typeof collectionsViews)[number];

export const routes = {
  today: "/today",
  session: (classGroupId: string, sessionDate: string) => `/today/${classGroupId}/${sessionDate}`,
  newPrivateLesson: (date: string) => `/today/private/new?date=${encodeURIComponent(date)}`,
  privateLesson: (privateLessonId: string) => `/today/private/${privateLessonId}`,
  editPrivateLesson: (privateLessonId: string) => `/today/private/${privateLessonId}/edit`,
  classes: "/classes",
  newClassGroup: (weekday?: string) =>
    weekday ? `/classes/new?weekday=${encodeURIComponent(weekday)}` : "/classes/new",
  classGroup: (classGroupId: string) => `/classes/${classGroupId}`,
  editClassGroup: (classGroupId: string) => `/classes/${classGroupId}/edit`,
  enrollStudent: (classGroupId: string) => `/classes/${classGroupId}/enroll`,
  students: "/students",
  registerClient: "/students/new",
  clientDetail: (tab: ClientTab, clientId: string) => `/${tab}/clients/${clientId}`,
  addStudent: (tab: ClientTab, clientId: string) => `/${tab}/clients/${clientId}/new-student`,
  sellClassPack: (tab: ClientTab, clientId: string) => `/${tab}/clients/${clientId}/sell-pack`,
  clientCounterSale: (tab: ClientTab, clientId: string) =>
    `/${tab}/clients/${clientId}/counter-sale`,
  clientNewClassPack: (tab: ClientTab, clientId: string) =>
    `/${tab}/clients/${clientId}/new-class-pack`,
  recordPayment: (tab: ClientTab, clientId: string, month: string) =>
    `/${tab}/clients/${clientId}/pay?month=${encodeURIComponent(month)}`,
  fees: "/fees",
  collections: (view: CollectionsView) => `/fees?view=${view}`,
  feesDefaultMonthlyFee: "/fees/monthly-fee",
  welcome: "/welcome",
  signIn: "/sign-in",
  signUp: "/sign-up",
  resetPassword: "/reset-password",
  forgotPassword: (email: string) =>
    email ? `/forgot-password?email=${encodeURIComponent(email)}` : "/forgot-password",
  settings: "/settings",
  businessSettings: "/settings/business",
  teamNotifications: "/today/notifications",
  brandSettings: "/settings/brand",
  chooseAccount: "/choose-account",
  achievementSettings: "/settings/achievements",
  defaultMonthlyFee: "/settings/monthly-fee",
  instructors: "/settings/instructors",
  newInstructor: "/settings/instructors/new",
  classesNewInstructor: "/classes/instructors/new",
  instructor: (instructorId: string) => `/settings/instructors/${instructorId}`,
  classPacks: "/settings/class-packs",
  newClassPack: "/settings/class-packs/new",
  classPack: (classPackId: string) => `/settings/class-packs/${classPackId}`,
  products: "/settings/products",
  newProduct: "/settings/products/new",
  product: (productId: string) => `/settings/products/${productId}`,
  orders: (tab: OrderTab) => `/${tab}/orders`,
  plan: (tab: PlanTab) => `/${tab}/plan`,
  newCounterSale: (tab: OrderTab) => `/${tab}/orders/new`,
  importExport: "/settings/import-export",
  announcements: "/settings/announcements",
  team: "/settings/team",
  inviteMember: "/settings/team/invite",
  teamMember: (memberId: string) => `/settings/team/${memberId}`,
  roles: "/settings/roles",
  newRole: (copyFromRoleKey?: string) =>
    copyFromRoleKey
      ? `/settings/roles/new?copyFrom=${encodeURIComponent(copyFromRoleKey)}`
      : "/settings/roles/new",
  role: (roleKey: string) => `/settings/roles/${roleKey}`,
  acceptInvitation: "/accept-invitation",
  acceptStudentAppInvitation: "/accept-student-invitation",
  studentApp: "/student-app",
  studentAppClasses: "/student-app/classes",
  studentAppNews: "/student-app/news",
  studentAppShop: "/student-app/shop",
  studentAppProduct: (productId: string) =>
    `/student-app/shop?productId=${encodeURIComponent(productId)}`,
  studentAppOrders: "/student-app/orders",
  branches: "/settings/branches",
  newBranch: "/settings/branches/new",
} as const;
