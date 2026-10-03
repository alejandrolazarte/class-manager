export type StockMode = "Unlimited" | "Tracked" | "TrackedWithBackorder";

export const stockModes: readonly StockMode[] = ["Tracked", "TrackedWithBackorder", "Unlimited"];

export interface ProductVariant {
  id: string;
  name: string;
  stock: number | null;
}

export interface Product {
  id: string;
  name: string;
  description: string | null;
  price: number;
  stockMode: StockMode;
  isVisibleInApp: boolean;
  isActive: boolean;
  variants: ProductVariant[];
  imageUrl: string | null;
}

export interface VariantChange {
  id: string | null;
  name: string;
}

export interface SaveProductRequest {
  name: string;
  description: string | null;
  price: number;
  stockMode: StockMode;
  isVisibleInApp: boolean;
  variants: VariantChange[];
}

export type StockMovementKind = "Restock" | "Adjustment" | "Sale" | "Refund";

export interface RecordStockMovementRequest {
  variantId: string;
  kind: StockMovementKind;
  quantity: number;
  note: string | null;
}

export interface StockMovement {
  id: string;
  variantId: string;
  variantName: string;
  kind: StockMovementKind;
  quantity: number;
  orderId: string | null;
  note: string | null;
  createdAt: string;
}
