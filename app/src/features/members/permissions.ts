export const permissions = {
  businessView: "business.view",
  businessManage: "business.manage",
  membersView: "members.view",
  membersManage: "members.manage",
  branchOwnersManage: "branchOwners.manage",
  rolesManage: "roles.manage",
  branchesCreate: "branches.create",
  brandOwnersManage: "brandOwners.manage",
  instructorsView: "instructors.view",
  instructorsManage: "instructors.manage",
  classGroupsViewOwn: "classGroups.view.own",
  classGroupsViewAll: "classGroups.view.all",
  classGroupsManage: "classGroups.manage",
  enrollmentsView: "enrollments.view",
  enrollmentsManageOwn: "enrollments.manage.own",
  enrollmentsManage: "enrollments.manage",
  sessionsViewOwn: "sessions.view.own",
  sessionsViewAll: "sessions.view.all",
  sessionsManage: "sessions.manage",
  attendanceRecordOwn: "attendance.record.own",
  attendanceRecordAll: "attendance.record.all",
  privateLessonsViewOwn: "privateLessons.view.own",
  privateLessonsViewAll: "privateLessons.view.all",
  privateLessonsManageOwn: "privateLessons.manage.own",
  privateLessonsManageAll: "privateLessons.manage.all",
  studentsViewOwn: "students.view.own",
  studentsViewAll: "students.view.all",
  studentsManage: "students.manage",
  paymentsViewOwn: "payments.view.own",
  paymentsViewAll: "payments.view.all",
  paymentsRecord: "payments.record",
  classPacksView: "classPacks.view",
  classPacksManage: "classPacks.manage",
  classPacksSell: "classPacks.sell",
  productsView: "products.view",
  productsManage: "products.manage",
  ordersViewOwn: "orders.view.own",
  ordersViewAll: "orders.view.all",
  ordersManage: "orders.manage",
  importExportRun: "importExport.run",
} as const;

export type Permission = (typeof permissions)[keyof typeof permissions];

export const everyPermission: readonly Permission[] = Object.values(permissions);

export const brandOnlyPermissions: readonly Permission[] = [
  permissions.branchOwnersManage,
  permissions.branchesCreate,
  permissions.brandOwnersManage,
];

export const everyInstructorPermissionByOwnPermission: Readonly<
  Partial<Record<Permission, Permission>>
> = {
  [permissions.classGroupsViewOwn]: permissions.classGroupsViewAll,
  [permissions.sessionsViewOwn]: permissions.sessionsViewAll,
  [permissions.attendanceRecordOwn]: permissions.attendanceRecordAll,
  [permissions.privateLessonsViewOwn]: permissions.privateLessonsViewAll,
  [permissions.privateLessonsManageOwn]: permissions.privateLessonsManageAll,
  [permissions.studentsViewOwn]: permissions.studentsViewAll,
  [permissions.paymentsViewOwn]: permissions.paymentsViewAll,
  [permissions.enrollmentsManageOwn]: permissions.enrollmentsManage,
  [permissions.ordersViewOwn]: permissions.ordersViewAll,
};

export function needsCoach(rolePermissions: readonly Permission[]): boolean {
  return Object.entries(everyInstructorPermissionByOwnPermission).some(
    ([ownPermission, allPermission]) =>
      rolePermissions.includes(ownPermission as Permission) &&
      !rolePermissions.includes(allPermission),
  );
}
