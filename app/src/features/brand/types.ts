import { SessionKind } from "@/features/authentication/sessionKind";

export interface Brand {
  displayName: string;
  brandName: string | null;
  themeColor: string | null;
  accentColor: string | null;
  locksTheme: boolean;
  logoUpdatedAt: string | null;
}

export interface UpdateBrandRequest {
  brandName: string | null;
  themeColor: string | null;
  accentColor: string | null;
  locksTheme: boolean;
}

export type BrandAudience = SessionKind;
