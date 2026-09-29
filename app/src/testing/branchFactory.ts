import { Branch } from "@/features/branches/types";

export function buildBranch(overrides: Partial<Branch> = {}): Branch {
  return {
    businessId: "branch-tenerife",
    name: "DF Tenerife",
    branchRole: "BranchOwner",
    isBrandOwner: true,
    isCurrent: true,
    ...overrides,
  };
}
