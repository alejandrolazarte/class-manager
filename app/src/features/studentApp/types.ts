import { CatalogImage } from "@/features/catalogImages/types";
import { FeeStatus } from "@/features/fees/types";

export interface StudentAppNextClass {
  name: string;
  date: string;
  startTime: string;
  endTime: string;
  instructorFullName: string | null;
  location: string | null;
  isPrivateLesson: boolean;
  isCancelled: boolean;
  classGroupId: string | null;
  absenceNotified: boolean;
  isMakeup: boolean;
  isPackBooking: boolean;
}

export interface StudentAppPushKey {
  publicKey: string | null;
}

export type MakeupReason = "Notice" | "Cancelled";

export interface StudentAppMakeupCredit {
  missedOn: string;
  reason: MakeupReason;
  expiresOn: string;
}

export interface StudentAppMakeupSlot {
  classGroupId: string;
  name: string;
  date: string;
  startTime: string;
  endTime: string;
  instructorFullName: string | null;
  location: string | null;
  spotsLeft: number;
  isBooked: boolean;
}

export interface StudentAppMakeups {
  credits: StudentAppMakeupCredit[];
  slots: StudentAppMakeupSlot[];
}

export interface StudentAppPackClasses {
  classesLeft: number;
  slots: StudentAppMakeupSlot[];
}

export type WeekAttendance = "Attended" | "Missed" | "NoClasses";

export interface StudentAppAttendanceWeek {
  weekStart: string;
  attendance: WeekAttendance;
}

export type Medal =
  "FirstClass" | "TenClasses" | "FourWeekStreak" | "LeveledUp" | "PerfectMonth" | "HundredClasses";

export interface StudentAppLevel {
  name: string;
  requiredClasses: number;
}

export interface StudentAppAttendance {
  streakWeeks: number;
  streakSince: string | null;
  attendedClasses: number;
  recentWeeks: StudentAppAttendanceWeek[];
  bestStreakWeeks: number;
  level: number;
  medals: Medal[];
}

export interface StudentAppFeedback {
  date: string;
  className: string;
  instructorFullName: string | null;
  text: string;
}

export interface AccountStudent {
  id: string;
  fullName: string;
  nextClasses: StudentAppNextClass[];
  attendance: StudentAppAttendance;
  latestFeedback: StudentAppFeedback | null;
}

export interface StudentAppMonthlyFee {
  month: string;
  fee: number | null;
  paid: number;
  balance: number;
  status: FeeStatus;
}

export interface StudentAppClassBalance {
  availableClasses: number;
  unpaidClasses: number;
}

export interface StudentAppBilling {
  kind: "BusinessFee" | "CustomFee" | "ClassPacks";
  monthlyFee: StudentAppMonthlyFee | null;
  classes: StudentAppClassBalance | null;
}

export interface StudentAppHome {
  businessName: string;
  currencyCode: string;
  clientFullName: string;
  students: AccountStudent[];
  billing: StudentAppBilling;
  levels: StudentAppLevel[];
}

export interface InviteStudentAppRequest {
  email: string;
  studentId?: string;
  birthDate?: string;
}

export interface StudentAppInvitation {
  id: string;
  email: string;
  expiresAt: string;
}

export type StockAvailability = "Available" | "OnOrder" | "SoldOut";

export interface StudentAppShopPack {
  id: string;
  name: string;
  description: string | null;
  classCount: number;
  price: number;
  validityMonths: number | null;
  images: CatalogImage[];
}

export interface StudentAppShopVariant {
  id: string;
  name: string;
  availability: StockAvailability;
}

export interface StudentAppShopProduct {
  id: string;
  name: string;
  description: string | null;
  price: number;
  variants: StudentAppShopVariant[];
  images: CatalogImage[];
}

export type DeliveryMethod = "Pickup" | "InClass";

export interface DeliveryClass {
  classGroupId: string;
  classGroupName: string;
  studentFullName: string;
}

export interface StudentAppShop {
  currencyCode: string;
  packs: StudentAppShopPack[];
  products: StudentAppShopProduct[];
  deliveryClasses: DeliveryClass[];
}

export type StudentAppOrderStatus = "Requested" | "Paid" | "Delivered" | "Cancelled";

export interface StudentAppOrderLine {
  id: string;
  kind: "ClassPack" | "Product";
  name: string;
  quantity: number;
  total: number;
  refundedQuantity: number;
}

export interface StudentAppOrder {
  id: string;
  number: number;
  status: StudentAppOrderStatus;
  awaitsPickup: boolean;
  total: number;
  refundedAmount: number;
  createdAt: string;
  paidOn: string | null;
  expiresAt: string;
  lines: StudentAppOrderLine[];
  delivery: DeliveryMethod;
  deliveryClassGroupName: string | null;
  isReady: boolean;
}

export interface StudentAppDelivery {
  delivery: DeliveryMethod;
  deliveryClassGroupId: string | null;
}

export interface PlaceStudentAppOrderLine {
  classPackId: string | null;
  productVariantId: string | null;
  quantity: number;
}

export type StudentAppNewsKind = "Announcement" | "OrderReady" | "CoachFeedback" | "ClassCancelled";

export interface StudentAppNewsItem {
  id: string;
  kind: StudentAppNewsKind;
  occurredAt: string;
  isUnread: boolean;
  title: string | null;
  body: string | null;
  studentFullNames: string[];
  className: string | null;
  instructorFullName: string | null;
  classDate: string | null;
  orderId: string | null;
}

export interface StudentAppNews {
  items: StudentAppNewsItem[];
  unreadCount: number;
}
