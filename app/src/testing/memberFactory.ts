import { everyPermission, Permission } from "@/features/members/permissions";
import { CurrentMember, Invitation, Member } from "@/features/members/types";

export const coachPermissions: readonly Permission[] = [
  "business.view",
  "instructors.view",
  "classGroups.view.own",
  "enrollments.view",
  "sessions.view.own",
  "attendance.record.own",
  "privateLessons.view.own",
  "privateLessons.manage.own",
  "students.view.own",
  "students.manage",
];

export const viewerPermissions: readonly Permission[] = [
  "business.view",
  "members.view",
  "instructors.view",
  "classGroups.view.all",
  "enrollments.view",
  "sessions.view.all",
  "privateLessons.view.all",
  "students.view.all",
  "payments.view.all",
  "classPacks.view",
];

export function buildCurrentMember(overrides: Partial<CurrentMember> = {}): CurrentMember {
  return {
    businessId: "business-1",
    branchRole: "BranchOwner",
    instructorId: null,
    isBrandOwner: true,
    permissions: [...everyPermission],
    ...overrides,
  };
}

export function buildCoach(instructorId = "instructor-coach"): CurrentMember {
  return buildCurrentMember({
    branchRole: "Coach",
    instructorId,
    isBrandOwner: false,
    permissions: [...coachPermissions],
  });
}

export function buildViewer(): CurrentMember {
  return buildCurrentMember({
    branchRole: "Viewer",
    isBrandOwner: false,
    permissions: [...viewerPermissions],
  });
}

export function buildMember(overrides: Partial<Member> = {}): Member {
  return {
    id: "member-1",
    fullName: "Marcos Díaz",
    email: "marcos@example.com",
    role: "Coach",
    customRoleId: null,
    instructorId: "instructor-coach",
    isCurrentUser: false,
    isBrandOwner: false,
    ...overrides,
  };
}

export function buildInvitation(overrides: Partial<Invitation> = {}): Invitation {
  return {
    id: "invitation-1",
    email: "lucia@example.com",
    role: "Viewer",
    customRoleId: null,
    instructorId: null,
    expiresAt: "2026-10-05T12:00:00Z",
    ...overrides,
  };
}
