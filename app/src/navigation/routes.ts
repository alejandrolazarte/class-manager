export const clientTabs = ["students", "fees"] as const;
export type ClientTab = (typeof clientTabs)[number];

export const orderTabs = ["settings", "today", "fees"] as const;
export type OrderTab = (typeof orderTabs)[number];

export const planTabs = ["settings", "today", "students"] as const;
export type PlanTab = (typeof planTabs)[number];

export const collectionsViews = ["fees", "orders"] as const;
export type CollectionsView = (typeof collectionsViews)[number];

export interface StudentAppClassRoute {
  classGroupId: string;
  date: string;
  startTime: string;
}

export const routes = {
  today: "/today",
  session: (classGroupId: string, sessionDate: string) => `/today/${classGroupId}/${sessionDate}`,
  newPrivateLesson: (date: string) => `/today/private/new?date=${encodeURIComponent(date)}`,
  privateLesson: (privateLessonId: string) => `/today/private/${privateLessonId}`,
  classMeasurements: (classGroupId: string, sessionDate: string) =>
    `/today/${classGroupId}/measurements?date=${encodeURIComponent(sessionDate)}`,
  editPrivateLesson: (privateLessonId: string) => `/today/private/${privateLessonId}/edit`,
  classes: "/classes",
  newClassGroup: (weekday?: string) =>
    weekday ? `/classes/new?weekday=${encodeURIComponent(weekday)}` : "/classes/new",
  classGroup: (classGroupId: string) => `/classes/${classGroupId}`,
  editClassGroup: (classGroupId: string) => `/classes/${classGroupId}/edit`,
  enrollStudent: (classGroupId: string) => `/classes/${classGroupId}/enroll`,
  students: "/students",
  registerClient: "/students/new",
  importStudents: "/students/import",
  clientDetail: (tab: ClientTab, clientId: string) => `/${tab}/clients/${clientId}`,
  addStudent: (tab: ClientTab, clientId: string) => `/${tab}/clients/${clientId}/new-student`,
  editClient: (tab: ClientTab, clientId: string) => `/${tab}/clients/${clientId}/edit`,
  editStudent: (tab: ClientTab, clientId: string, studentId: string) =>
    `/${tab}/clients/${clientId}/edit-student?studentId=${encodeURIComponent(studentId)}`,
  separateClient: (tab: ClientTab, clientId: string) => `/${tab}/clients/${clientId}/separate`,
  separateStudent: (tab: ClientTab, clientId: string, studentId: string) =>
    `/${tab}/clients/${clientId}/separate?studentId=${encodeURIComponent(studentId)}`,
  clientBillingPlan: (tab: ClientTab, clientId: string) =>
    `/${tab}/clients/${clientId}/billing-plan`,
  sellClassPack: (tab: ClientTab, clientId: string) => `/${tab}/clients/${clientId}/sell-pack`,
  clientCounterSale: (tab: ClientTab, clientId: string) =>
    `/${tab}/clients/${clientId}/counter-sale`,
  clientNewClassPack: (tab: ClientTab, clientId: string) =>
    `/${tab}/clients/${clientId}/new-class-pack`,
  studentMeasurement: (
    tab: ClientTab,
    clientId: string,
    studentId: string,
    measurementId: string,
  ) => `/${tab}/clients/${clientId}/measurements/${studentId}/${measurementId}`,
  recordMeasurement: (
    tab: ClientTab,
    clientId: string,
    studentId: string,
    measurementId?: string,
  ) =>
    measurementId
      ? `/${tab}/clients/${clientId}/record-measurement?studentId=${encodeURIComponent(studentId)}&measurementId=${encodeURIComponent(measurementId)}`
      : `/${tab}/clients/${clientId}/record-measurement?studentId=${encodeURIComponent(studentId)}`,
  recordPayment: (tab: ClientTab, clientId: string, month: string) =>
    `/${tab}/clients/${clientId}/pay?month=${encodeURIComponent(month)}`,
  fees: "/fees",
  collections: (view: CollectionsView) => `/fees?view=${view}`,
  feesDefaultMonthlyFee: "/fees/monthly-fee",
  welcome: "/welcome",
  signIn: "/sign-in",
  signUp: "/sign-up",
  legalDocument: (kind: "notice" | "terms" | "privacy") => `/legal/${kind}`,
  resetPassword: "/reset-password",
  appEntry: "/",
  myProfile: "/settings/profile",
  appFeedback: "/settings/feedback",
  studentAppProfile: "/student-app/profile",
  studentAppFamily: "/student-app/family",
  studentAppEditProfile: "/student-app/edit-profile",
  studentAppFeedback: "/student-app/feedback",
  editMyProfile: "/settings/edit-profile",
  changeMyEmail: "/settings/change-email",
  studentAppChangeEmail: "/student-app/change-email",
  deleteMyAccount: "/settings/delete-account",
  studentAppDeleteAccount: "/student-app/delete-account",
  myPasskeys: "/settings/passkeys",
  addMyPasskey: "/settings/add-passkey",
  studentAppPasskeys: "/student-app/passkeys",
  studentAppAddPasskey: "/student-app/add-passkey",
  forgotPassword: (email: string) =>
    email ? `/forgot-password?email=${encodeURIComponent(email)}` : "/forgot-password",
  settings: "/settings",
  businessSettings: "/settings/business",
  teamNotifications: "/today/notifications",
  brandSettings: "/settings/brand",
  chooseAccount: "/choose-account",
  achievementSettings: "/settings/achievements",
  measurementSettings: "/settings/measurements",
  newMeasurement: "/settings/measurements/new",
  defaultMonthlyFee: "/settings/monthly-fee",
  instructors: "/settings/instructors",
  newInstructor: "/settings/instructors/new",
  importInstructors: "/settings/instructors/import",
  classesNewInstructor: "/classes/instructors/new",
  instructor: (instructorId: string) => `/settings/instructors/${instructorId}`,
  editInstructor: (instructorId: string) => `/settings/instructors/${instructorId}/edit`,
  separateInstructor: (instructorId: string) => `/settings/instructors/${instructorId}/separate`,
  classPacks: "/settings/class-packs",
  newClassPack: "/settings/class-packs/new",
  classPack: (classPackId: string) => `/settings/class-packs/${classPackId}`,
  products: "/settings/products",
  newProduct: "/settings/products/new",
  product: (productId: string) => `/settings/products/${productId}`,
  orders: (tab: OrderTab) => `/${tab}/orders`,
  plan: (tab: PlanTab) => `/${tab}/plan`,
  newCounterSale: (tab: OrderTab) => `/${tab}/orders/new`,
  order: (tab: OrderTab, orderId: string) => `/${tab}/orders/${orderId}`,
  announcements: "/settings/announcements",
  newAnnouncement: "/settings/announcements/new",
  announcement: (announcementId: string) => `/settings/announcements/${announcementId}`,
  editAnnouncement: (announcementId: string) => `/settings/announcements/${announcementId}/edit`,
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
  authorizeStudentApp: "/authorize-student-app",
  studentApp: "/student-app",
  studentAppClasses: "/student-app/classes",
  studentAppNews: "/student-app/news",
  studentAppShop: "/student-app/shop",
  studentAppProduct: (productId: string) =>
    `/student-app/shop?productId=${encodeURIComponent(productId)}`,
  studentAppOrders: "/student-app/orders",
  studentAppLevels: "/student-app/levels",
  studentAppStreak: "/student-app/streak",
  studentAppMeasurement: (studentId: string, measurementId: string) =>
    `/student-app/measurement?studentId=${encodeURIComponent(studentId)}&measurementId=${encodeURIComponent(measurementId)}`,
  studentAppRecordMeasurement: (studentId: string) =>
    `/student-app/record-measurement?studentId=${encodeURIComponent(studentId)}`,
  studentAppClass: ({ classGroupId, date, startTime }: StudentAppClassRoute) =>
    `/student-app/class?classGroupId=${encodeURIComponent(classGroupId)}&date=${date}&startTime=${encodeURIComponent(startTime)}`,
  branches: "/settings/branches",
  newBranch: "/settings/branches/new",
} as const;
