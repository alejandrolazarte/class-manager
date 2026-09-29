import { BusinessRole } from "@/features/members/types";

export interface Branch {
  businessId: string;
  name: string;
  branchRole: BusinessRole | null;
  isBrandOwner: boolean;
  isCurrent: boolean;
  customRoleName?: string | null;
}

export interface CreateBranchRequest {
  name: string;
  timeZoneId: string;
  currencyCode: string;
  defaultCountryCallingCode: string;
}
