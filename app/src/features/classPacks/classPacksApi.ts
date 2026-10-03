import { PickedFile, toFileForm } from "@/api/fileForm";
import { httpClient } from "@/api/httpClient";
import {
  ClassBalance,
  ClassPack,
  ClassPackPurchase,
  SaveClassPackRequest,
  SellClassPackRequest,
} from "@/features/classPacks/types";

const classPacksPath = "/api/class-packs";
const clientsPath = "/api/clients";
const classPackPurchasesPath = "/api/class-pack-purchases";
const activeSegment = "active";
const imageSegment = "image";
const purchasesSegment = "class-pack-purchases";
const classBalanceSegment = "class-balance";
const includeInactiveParameters = { includeInactive: "true" };

function classPackPath(classPackId: string): string {
  return `${classPacksPath}/${encodeURIComponent(classPackId)}`;
}

function clientPath(clientId: string): string {
  return `${clientsPath}/${encodeURIComponent(clientId)}`;
}

export function listClassPacks(includeInactive: boolean): Promise<ClassPack[]> {
  return includeInactive
    ? httpClient.get<ClassPack[]>(classPacksPath, includeInactiveParameters)
    : httpClient.get<ClassPack[]>(classPacksPath);
}

export function createClassPack(request: SaveClassPackRequest): Promise<ClassPack> {
  return httpClient.post<ClassPack>(classPacksPath, request);
}

export function updateClassPack(
  classPackId: string,
  request: SaveClassPackRequest,
): Promise<ClassPack> {
  return httpClient.put<ClassPack>(classPackPath(classPackId), request);
}

export function setClassPackActive(classPackId: string, isActive: boolean): Promise<ClassPack> {
  return httpClient.put<ClassPack>(`${classPackPath(classPackId)}/${activeSegment}`, {
    isActive,
  });
}

export function uploadClassPackImage(classPackId: string, file: PickedFile): Promise<ClassPack> {
  return httpClient.putForm<ClassPack>(
    `${classPackPath(classPackId)}/${imageSegment}`,
    toFileForm(file),
  );
}

export function removeClassPackImage(classPackId: string): Promise<ClassPack> {
  return httpClient.delete<ClassPack>(`${classPackPath(classPackId)}/${imageSegment}`);
}

export function sellClassPack(
  clientId: string,
  request: SellClassPackRequest,
): Promise<ClassPackPurchase> {
  return httpClient.post<ClassPackPurchase>(`${clientPath(clientId)}/${purchasesSegment}`, request);
}

export function deleteClassPackPurchase(purchaseId: string): Promise<void> {
  return httpClient.delete<void>(`${classPackPurchasesPath}/${encodeURIComponent(purchaseId)}`);
}

export function getClassBalance(clientId: string): Promise<ClassBalance> {
  return httpClient.get<ClassBalance>(`${clientPath(clientId)}/${classBalanceSegment}`);
}
