import { BrandAudience } from "@/features/brand/types";

export const brandQueryKeys = {
  all: ["brand"] as const,
  current: (audience: BrandAudience) => [...brandQueryKeys.all, audience] as const,
  logo: (audience: BrandAudience, logoUpdatedAt: string | null) =>
    [...brandQueryKeys.all, audience, "logo", logoUpdatedAt] as const,
};
