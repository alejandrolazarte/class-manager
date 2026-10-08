import { PickedFile, toFileForm } from "@/api/fileForm";
import { httpClient } from "@/api/httpClient";
import { ClassGroup, SaveClassGroupRequest } from "@/features/classGroups/types";

const classGroupsPath = "/api/class-groups";
const activeSegment = "active";
const materialSegment = "material";
const includeInactiveParameters = { includeInactive: "true" };

function classGroupPath(classGroupId: string): string {
  return `${classGroupsPath}/${encodeURIComponent(classGroupId)}`;
}

export function listActiveClassGroups(): Promise<ClassGroup[]> {
  return httpClient.get<ClassGroup[]>(classGroupsPath);
}

export function listClassGroupsIncludingInactive(): Promise<ClassGroup[]> {
  return httpClient.get<ClassGroup[]>(classGroupsPath, includeInactiveParameters);
}

export function createClassGroup(request: SaveClassGroupRequest): Promise<ClassGroup> {
  return httpClient.post<ClassGroup>(classGroupsPath, request);
}

export function updateClassGroup(
  classGroupId: string,
  request: SaveClassGroupRequest,
): Promise<ClassGroup> {
  return httpClient.put<ClassGroup>(classGroupPath(classGroupId), request);
}

export function setClassGroupActive(classGroupId: string, isActive: boolean): Promise<ClassGroup> {
  return httpClient.put<ClassGroup>(`${classGroupPath(classGroupId)}/${activeSegment}`, {
    isActive,
  });
}

export function uploadClassMaterialFile(
  classGroupId: string,
  file: PickedFile,
): Promise<ClassGroup> {
  return httpClient.postForm<ClassGroup>(
    `${classGroupPath(classGroupId)}/${materialSegment}`,
    toFileForm(file),
  );
}

export function removeClassMaterialFile(classGroupId: string): Promise<ClassGroup> {
  return httpClient.delete<ClassGroup>(`${classGroupPath(classGroupId)}/${materialSegment}`);
}
