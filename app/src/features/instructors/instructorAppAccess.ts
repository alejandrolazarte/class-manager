import { InstructorAppAccessStatus } from "@/features/instructors/types";
import { Invitation, Member, Team } from "@/features/members/types";

export interface InstructorAppAccess {
  status: InstructorAppAccessStatus;
  member?: Member;
  invitation?: Invitation;
}

export function instructorAppAccess(
  instructorId: string,
  team: Team | undefined,
): InstructorAppAccess {
  const member = team?.members.find((candidate) => candidate.instructorId === instructorId);
  if (member !== undefined) {
    return { status: "Active", member };
  }
  const invitation = team?.invitations.find((candidate) => candidate.instructorId === instructorId);
  if (invitation !== undefined) {
    return { status: "Invited", invitation };
  }
  return { status: "NotInvited" };
}
