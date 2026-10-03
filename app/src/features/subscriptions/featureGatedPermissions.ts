import { Permission, permissions } from "@/features/members/permissions";
import { FeatureCode, featureCodes } from "@/features/subscriptions/subscriptionCodes";

export const featureByPermission: Readonly<Partial<Record<Permission, FeatureCode>>> = {
  [permissions.importExportRun]: featureCodes.importExport,
  [permissions.rolesManage]: featureCodes.customRoles,
  [permissions.classPacksManage]: featureCodes.classPacks,
  [permissions.classPacksSell]: featureCodes.classPacks,
  [permissions.privateLessonsManageOwn]: featureCodes.classPacks,
  [permissions.privateLessonsManageAll]: featureCodes.classPacks,
  [permissions.productsManage]: featureCodes.shop,
  [permissions.ordersManage]: featureCodes.shop,
};
