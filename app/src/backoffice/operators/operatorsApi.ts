import { getBackofficeJson, sendBackofficeJson } from "@/backoffice/api/backofficeHttpClient";

const operatorsPath = "/api/operators";

export const operatorsQueryKey = ["backofficeOperators"];

export type PlatformRole = "PlatformOperator" | "PlatformReader";

export const platformRoles: readonly PlatformRole[] = ["PlatformOperator", "PlatformReader"];

export interface BackofficeOperator {
  loginId: string;
  provider: string;
  email: string;
  fullName: string | null;
  role: PlatformRole | null;
  branchName: string | null;
  enabledAt: string;
  isCurrentOperator: boolean;
}

export interface AccessRequest {
  id: string;
  provider: string;
  email: string;
  code: string;
  requestedAt: string;
}

export interface PlatformTeamMember {
  memberId: string;
  fullName: string;
  email: string | null;
  branchName: string;
  role: string;
}

export interface OperatorList {
  operators: BackofficeOperator[];
  accessRequests: AccessRequest[];
  teamMembers: PlatformTeamMember[];
}

export interface EnableOperatorRequest {
  accessRequestId: string;
  memberId: string;
  role: PlatformRole;
}

export function getOperators(): Promise<OperatorList> {
  return getBackofficeJson<OperatorList>(operatorsPath);
}

export function enableOperator(request: EnableOperatorRequest): Promise<{ loginId: string }> {
  return sendBackofficeJson("POST", operatorsPath, request);
}

export function disableOperator(loginId: string): Promise<{ loginId: string }> {
  return sendBackofficeJson("DELETE", `${operatorsPath}/${encodeURIComponent(loginId)}`);
}
