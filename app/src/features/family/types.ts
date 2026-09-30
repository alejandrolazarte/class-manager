import { FeeStatus } from "@/features/fees/types";

export interface FamilyNextClass {
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
}

export interface FamilyPushKey {
  publicKey: string | null;
}

export type MakeupReason = "Notice" | "Cancelled";

export interface FamilyMakeupCredit {
  missedOn: string;
  reason: MakeupReason;
  expiresOn: string;
}

export interface FamilyMakeupSlot {
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

export interface FamilyMakeups {
  credits: FamilyMakeupCredit[];
  slots: FamilyMakeupSlot[];
}

export type WeekAttendance = "Attended" | "Missed" | "NoClasses";

export interface FamilyAttendanceWeek {
  weekStart: string;
  attendance: WeekAttendance;
}

export type Medal =
  "FirstClass" | "TenClasses" | "FourWeekStreak" | "LeveledUp" | "PerfectMonth" | "HundredClasses";

export interface FamilyLevel {
  name: string;
  requiredClasses: number;
}

export interface FamilyAttendance {
  streakWeeks: number;
  streakSince: string | null;
  attendedClasses: number;
  recentWeeks: FamilyAttendanceWeek[];
  bestStreakWeeks: number;
  level: number;
  medals: Medal[];
}

export interface FamilyFeedback {
  date: string;
  className: string;
  instructorFullName: string | null;
  text: string;
}

export interface FamilyStudent {
  id: string;
  fullName: string;
  nextClasses: FamilyNextClass[];
  attendance: FamilyAttendance;
  latestFeedback: FamilyFeedback | null;
}

export interface FamilyMonthlyFee {
  month: string;
  fee: number | null;
  paid: number;
  balance: number;
  status: FeeStatus;
}

export interface FamilyClassBalance {
  availableClasses: number;
  unpaidClasses: number;
}

export interface FamilyBilling {
  kind: "BusinessFee" | "CustomFee" | "ClassPacks";
  monthlyFee: FamilyMonthlyFee | null;
  classes: FamilyClassBalance | null;
}

export interface FamilyHome {
  businessName: string;
  currencyCode: string;
  clientFullName: string;
  students: FamilyStudent[];
  billing: FamilyBilling;
  levels: FamilyLevel[];
}

export interface InviteFamilyRequest {
  email: string;
}

export interface FamilyInvitation {
  id: string;
  email: string;
  expiresAt: string;
}

export type StockAvailability = "Available" | "OnOrder" | "SoldOut";

export interface FamilyShopPack {
  id: string;
  name: string;
  classCount: number;
  price: number;
  validityMonths: number | null;
}

export interface FamilyShopVariant {
  id: string;
  name: string;
  availability: StockAvailability;
}

export interface FamilyShopProduct {
  id: string;
  name: string;
  description: string | null;
  price: number;
  variants: FamilyShopVariant[];
}

export type DeliveryMethod = "Pickup" | "InClass";

export interface DeliveryClass {
  classGroupId: string;
  classGroupName: string;
  studentFullName: string;
}

export interface FamilyShop {
  currencyCode: string;
  packs: FamilyShopPack[];
  products: FamilyShopProduct[];
  deliveryClasses: DeliveryClass[];
}

export type FamilyOrderStatus = "Requested" | "Paid" | "Delivered" | "Cancelled";

export interface FamilyOrderLine {
  id: string;
  kind: "ClassPack" | "Product";
  name: string;
  quantity: number;
  total: number;
  refundedQuantity: number;
}

export interface FamilyOrder {
  id: string;
  status: FamilyOrderStatus;
  awaitsPickup: boolean;
  total: number;
  refundedAmount: number;
  createdAt: string;
  paidOn: string | null;
  expiresAt: string;
  lines: FamilyOrderLine[];
  delivery: DeliveryMethod;
  deliveryClassGroupName: string | null;
  isReady: boolean;
}

export interface FamilyDelivery {
  delivery: DeliveryMethod;
  deliveryClassGroupId: string | null;
}

export interface PlaceFamilyOrderLine {
  classPackId: string | null;
  productVariantId: string | null;
  quantity: number;
}

export type FamilyNewsKind = "Announcement" | "OrderReady" | "CoachFeedback" | "ClassCancelled";

export interface FamilyNewsItem {
  id: string;
  kind: FamilyNewsKind;
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

export interface FamilyNews {
  items: FamilyNewsItem[];
  unreadCount: number;
}
