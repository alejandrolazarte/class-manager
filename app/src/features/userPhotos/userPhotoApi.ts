import { PickedFile, toFileForm } from "@/api/fileForm";
import { httpClient } from "@/api/httpClient";
import { MyPhoto, PhotoApproval } from "@/features/userPhotos/types";

const myPhotoPath = "/api/me/photo";
const photoApprovalsPath = "/api/student-app/photo-approvals";
const authorizationSegment = "authorization";
const refusalSegment = "refusal";

export function getMyPhoto(): Promise<MyPhoto> {
  return httpClient.get<MyPhoto>(myPhotoPath);
}

export function changeMyPhoto(file: PickedFile): Promise<MyPhoto> {
  return httpClient.putForm<MyPhoto>(myPhotoPath, toFileForm(file));
}

export function removeMyPhoto(): Promise<void> {
  return httpClient.delete<void>(myPhotoPath);
}

export function listPhotoApprovals(): Promise<PhotoApproval[]> {
  return httpClient.get<PhotoApproval[]>(photoApprovalsPath);
}

export function approvePhoto(photoId: string): Promise<void> {
  return httpClient.post<void>(
    `${photoApprovalsPath}/${encodeURIComponent(photoId)}/${authorizationSegment}`,
    {},
  );
}

export function refusePhoto(photoId: string): Promise<void> {
  return httpClient.post<void>(
    `${photoApprovalsPath}/${encodeURIComponent(photoId)}/${refusalSegment}`,
    {},
  );
}
