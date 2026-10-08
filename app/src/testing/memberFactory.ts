import { everyPermission, Permission } from "@/features/members/permissions";
import { CurrentMember, Invitation, Member } from "@/features/members/types";

export const instructorPermissions: readonly Permission[] = [
  "business.view",
  "instructors.view",
  "classGroups.view.own",
  "enrollments.view",
  "enrollments.manage.own",
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
  "products.view",
  "orders.view.all",
];

export function buildCurrentMember(overrides: Partial<CurrentMember> = {}): CurrentMember {
  return {
    businessId: "business-1",
    userId: "user-owner",
    branchRole: "BranchOwner",
    instructorId: null,
    isBrandOwner: true,
    permissions: [...everyPermission],
    ...overrides,
  };
}

export function buildInstructorMember(instructorId = "instructor-of-member"): CurrentMember {
  return buildCurrentMember({
    branchRole: "Instructor",
    instructorId,
    isBrandOwner: false,
    permissions: [...instructorPermissions],
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
    role: "Instructor",
    customRoleId: null,
    instructorId: "instructor-of-member",
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
