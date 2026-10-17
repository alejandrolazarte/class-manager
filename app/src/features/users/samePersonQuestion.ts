import { isApiError } from "@/api/httpClient";
import { getStringExtension } from "@/api/problemDetails";
import { userErrorCodes } from "@/features/users/userErrorCodes";

const personRoles = ["Client", "Student", "Instructor", "Team"] as const;

export type PersonRole = (typeof personRoles)[number];

export interface SamePersonQuestion {
  userId: string;
  fullName: string;
  roles: PersonRole[];
}

const userIdExtension = "userId";
const fullNameExtension = "fullName";
const rolesExtension = "roles";

function isPersonRole(role: unknown): role is PersonRole {
  return personRoles.some((personRole) => personRole === role);
}

export function toSamePersonQuestion(error: unknown): SamePersonQuestion | null {
  if (!isApiError(error) || !error.hasCode(userErrorCodes.samePerson)) {
    return null;
  }
  const userId = getStringExtension(error.problem, userIdExtension);
  const fullName = getStringExtension(error.problem, fullNameExtension);
  const roles = error.problem[rolesExtension];
  if (userId === undefined || fullName === undefined) {
    return null;
  }
  return { userId, fullName, roles: Array.isArray(roles) ? roles.filter(isPersonRole) : [] };
}
