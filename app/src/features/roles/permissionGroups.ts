import { Permission, permissions } from "@/features/members/permissions";
import { TranslationKey } from "@/i18n/translate";

export type PermissionItem =
  | { kind: "toggle"; labelKey: TranslationKey; permission: Permission }
  | { kind: "scope"; labelKey: TranslationKey; own: Permission; all: Permission };

export interface PermissionGroup {
  titleKey: TranslationKey;
  items: PermissionItem[];
}

export const permissionGroups: PermissionGroup[] = [
  {
    titleKey: "roles.group.business",
    items: [
      {
        kind: "toggle",
        labelKey: "roles.permission.businessView",
        permission: permissions.businessView,
      },
      {
        kind: "toggle",
        labelKey: "roles.permission.businessManage",
        permission: permissions.businessManage,
      },
    ],
  },
  {
    titleKey: "roles.group.team",
    items: [
      {
        kind: "toggle",
        labelKey: "roles.permission.membersView",
        permission: permissions.membersView,
      },
      {
        kind: "toggle",
        labelKey: "roles.permission.membersManage",
        permission: permissions.membersManage,
      },
      {
        kind: "toggle",
        labelKey: "roles.permission.rolesManage",
        permission: permissions.rolesManage,
      },
    ],
  },
  {
    titleKey: "roles.group.instructors",
    items: [
      {
        kind: "toggle",
        labelKey: "roles.permission.instructorsView",
        permission: permissions.instructorsView,
      },
      {
        kind: "toggle",
        labelKey: "roles.permission.instructorsManage",
        permission: permissions.instructorsManage,
      },
    ],
  },
  {
    titleKey: "roles.group.classes",
    items: [
      {
        kind: "scope",
        labelKey: "roles.permission.classGroupsView",
        own: permissions.classGroupsViewOwn,
        all: permissions.classGroupsViewAll,
      },
      {
        kind: "toggle",
        labelKey: "roles.permission.classGroupsManage",
        permission: permissions.classGroupsManage,
      },
      {
        kind: "toggle",
        labelKey: "roles.permission.enrollmentsView",
        permission: permissions.enrollmentsView,
      },
      {
        kind: "scope",
        labelKey: "roles.permission.enrollmentsManage",
        own: permissions.enrollmentsManageOwn,
        all: permissions.enrollmentsManage,
      },
    ],
  },
  {
    titleKey: "roles.group.agenda",
    items: [
      {
        kind: "scope",
        labelKey: "roles.permission.sessionsView",
        own: permissions.sessionsViewOwn,
        all: permissions.sessionsViewAll,
      },
      {
        kind: "toggle",
        labelKey: "roles.permission.sessionsManage",
        permission: permissions.sessionsManage,
      },
      {
        kind: "scope",
        labelKey: "roles.permission.attendanceRecord",
        own: permissions.attendanceRecordOwn,
        all: permissions.attendanceRecordAll,
      },
    ],
  },
  {
    titleKey: "roles.group.privateLessons",
    items: [
      {
        kind: "scope",
        labelKey: "roles.permission.privateLessonsView",
        own: permissions.privateLessonsViewOwn,
        all: permissions.privateLessonsViewAll,
      },
      {
        kind: "scope",
        labelKey: "roles.permission.privateLessonsManage",
        own: permissions.privateLessonsManageOwn,
        all: permissions.privateLessonsManageAll,
      },
    ],
  },
  {
    titleKey: "roles.group.students",
    items: [
      {
        kind: "scope",
        labelKey: "roles.permission.studentsView",
        own: permissions.studentsViewOwn,
        all: permissions.studentsViewAll,
      },
      {
        kind: "toggle",
        labelKey: "roles.permission.studentsManage",
        permission: permissions.studentsManage,
      },
    ],
  },
  {
    titleKey: "roles.group.money",
    items: [
      {
        kind: "scope",
        labelKey: "roles.permission.paymentsView",
        own: permissions.paymentsViewOwn,
        all: permissions.paymentsViewAll,
      },
      {
        kind: "toggle",
        labelKey: "roles.permission.paymentsRecord",
        permission: permissions.paymentsRecord,
      },
      {
        kind: "toggle",
        labelKey: "roles.permission.classPacksView",
        permission: permissions.classPacksView,
      },
      {
        kind: "toggle",
        labelKey: "roles.permission.classPacksManage",
        permission: permissions.classPacksManage,
      },
      {
        kind: "toggle",
        labelKey: "roles.permission.classPacksSell",
        permission: permissions.classPacksSell,
      },
    ],
  },
  {
    titleKey: "roles.group.data",
    items: [
      {
        kind: "toggle",
        labelKey: "roles.permission.importExportRun",
        permission: permissions.importExportRun,
      },
    ],
  },
];

export function permissionsOf(item: PermissionItem): Permission[] {
  return item.kind === "toggle" ? [item.permission] : [item.own, item.all];
}
