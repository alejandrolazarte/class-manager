import { DeliveryClass } from "@/features/studentApp/types";

export interface DeliveryClassGroup {
  classGroupId: string;
  classGroupName: string;
  studentNames: string[];
}

export function groupDeliveryClasses(
  deliveryClasses: readonly DeliveryClass[],
): DeliveryClassGroup[] {
  const groups = new Map<string, DeliveryClassGroup>();
  for (const deliveryClass of deliveryClasses) {
    const group = groups.get(deliveryClass.classGroupId) ?? {
      classGroupId: deliveryClass.classGroupId,
      classGroupName: deliveryClass.classGroupName,
      studentNames: [],
    };
    group.studentNames.push(deliveryClass.studentFullName);
    groups.set(deliveryClass.classGroupId, group);
  }
  return [...groups.values()];
}
