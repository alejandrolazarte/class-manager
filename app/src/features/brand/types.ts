import { SessionKind } from "@/features/authentication/sessionKind";

export interface Brand {
  displayName: string;
  brandName: string | null;
  themeColor: string | null;
  accentColor: string | null;
  logoUpdatedAt: string | null;
}

export interface UpdateBrandRequest {
  brandName: string | null;
  themeColor: string | null;
  accentColor: string | null;
}

export type BrandAudience = SessionKind;
