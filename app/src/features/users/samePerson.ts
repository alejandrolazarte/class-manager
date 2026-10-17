import { isApiError } from "@/api/httpClient";
import { getStringExtension } from "@/api/problemDetails";
import { userErrorCodes } from "@/features/users/userErrorCodes";

const samePersonUserIdExtension = "samePersonUserId";
const samePersonFullNameExtension = "samePersonFullName";

export interface SamePerson {
  userId: string;
  fullName: string;
  email: string;
}

export function samePersonOf(requestError: unknown, email: string): SamePerson | null {
  if (!isApiError(requestError) || !requestError.hasCode(userErrorCodes.samePerson)) {
    return null;
  }
  const userId = getStringExtension(requestError.problem, samePersonUserIdExtension);
  const fullName = getStringExtension(requestError.problem, samePersonFullNameExtension);
  return userId && fullName ? { userId, fullName, email } : null;
}
