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
}

export interface FamilyStudent {
  id: string;
  fullName: string;
  nextClasses: FamilyNextClass[];
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
