import { httpClient } from "@/api/httpClient";
import {
  Business,
  UpdateBusinessSettingsRequest,
  UpdateBusinessSettingsResponse,
} from "@/features/business/types";

const currentBusinessPath = "/api/business";

export function getCurrentBusiness(): Promise<Business> {
  return httpClient.get<Business>(currentBusinessPath);
}

export function updateBusinessSettings(
  request: UpdateBusinessSettingsRequest,
): Promise<UpdateBusinessSettingsResponse> {
  return httpClient.put<UpdateBusinessSettingsResponse>(currentBusinessPath, request);
}
