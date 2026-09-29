import { httpClient } from "@/api/httpClient";
import { FamilyHome, FamilyInvitation, InviteFamilyRequest } from "@/features/family/types";

const familyPath = "/api/family";
const clientsPath = "/api/clients";
const appInvitationSegment = "app-invitation";

export function getFamilyHome(): Promise<FamilyHome> {
  return httpClient.get<FamilyHome>(familyPath);
}

export function inviteFamily(
  clientId: string,
  request: InviteFamilyRequest,
): Promise<FamilyInvitation> {
  return httpClient.post<FamilyInvitation>(
    `${clientsPath}/${encodeURIComponent(clientId)}/${appInvitationSegment}`,
    request,
  );
}
