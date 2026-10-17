import { ApiError } from "@/api/httpClient";
import { PersonRole } from "@/features/users/samePersonQuestion";
import { userErrorCodes } from "@/features/users/userErrorCodes";

const conflictStatus = 409;

export function buildSamePersonError(userId: string, fullName: string, roles: PersonRole[]) {
  return new ApiError(conflictStatus, {
    code: userErrorCodes.samePerson,
    userId,
    fullName,
    roles,
  });
}
