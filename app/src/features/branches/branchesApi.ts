import { httpClient } from "@/api/httpClient";
import { Branch, CreateBranchRequest } from "@/features/branches/types";

const myBranchesPath = "/api/me/branches";
const organizationBranchesPath = "/api/organization/branches";

export function listBranches(): Promise<Branch[]> {
  return httpClient.get<Branch[]>(myBranchesPath);
}

export function createBranch(request: CreateBranchRequest): Promise<Branch> {
  return httpClient.post<Branch>(organizationBranchesPath, request);
}
