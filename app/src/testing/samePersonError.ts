import { ApiError } from "@/api/httpClient";
import { userErrorCodes } from "@/features/users/userErrorCodes";

const conflictStatus = 409;

export function buildSamePersonError(samePersonUserId: string, samePersonFullName: string) {
  return new ApiError(conflictStatus, {
    code: userErrorCodes.samePerson,
    samePersonUserId,
    samePersonFullName,
  });
}
