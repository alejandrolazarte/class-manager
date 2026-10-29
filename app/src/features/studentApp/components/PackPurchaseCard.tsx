import { useState } from "react";
import { View } from "react-native";
import { PackClassRow } from "@/features/studentApp/components/PackClassRow";
import { dayAndMonthLabel } from "@/features/studentApp/studentAppSchedule";
import { StudentAppPackPurchase, StudentAppPackPurchaseStatus } from "@/features/studentApp/types";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { OutlinedBlock } from "@/ui/OutlinedBlock";
import { ProgressBar } from "@/ui/ProgressBar";
import { StatusPill, StatusTone } from "@/ui/StatusPill";
import { TextLink } from "@/ui/TextLink";

interface PackPurchaseCardProps {
  purchase: StudentAppPackPurchase;
  studentNameOf: (studentId: string | null) => string | null;
}

const statusTones: Record<StudentAppPackPurchaseStatus, StatusTone> = {
  Active: "success",
  UsedUp: "neutral",
  Expired: "neutral",
};

const statusLabelKeys = {
  Active: "student.classPacks.status.active",
  UsedUp: "student.classPacks.status.usedUp",
  Expired: "student.classPacks.status.expired",
} as const satisfies Record<StudentAppPackPurchaseStatus, string>;

const classGroupNameSeparator = ", ";
const detailSeparator = " · ";

function includedClassesOf(purchase: StudentAppPackPurchase): string {
  if (purchase.isForPrivateLessons) {
    return translate("student.classPacks.includesPrivateLessons");
  }
  if (purchase.coversAnyGroupClass) {
    return translate("student.classPacks.includesAnyClass");
  }
  return translate("student.classPacks.includes", {
    classes: purchase.classGroupNames.join(classGroupNameSeparator),
  });
}

function datesOf(purchase: StudentAppPackPurchase): string {
  const purchasedOn = translate("student.classPacks.purchasedOn", {
    date: dayAndMonthLabel(purchase.purchasedOn),
  });
  return purchase.expiresOn === null
    ? purchasedOn
    : [
        purchasedOn,
        translate("student.classPacks.expiresOn", { date: dayAndMonthLabel(purchase.expiresOn) }),
      ].join(detailSeparator);
}

export function PackPurchaseCard({ purchase, studentNameOf }: PackPurchaseCardProps) {
  const [isShowingClasses, setIsShowingClasses] = useState(false);
  const isActive = purchase.status === "Active";
  return (
    <OutlinedBlock className="gap-2">
      <View className="flex-row items-center justify-between gap-2">
        <AppText variant="heading" tone={isActive ? "default" : "muted"} className="min-w-0 flex-1">
          {purchase.name}
        </AppText>
        <StatusPill
          isSmall
          label={translate(statusLabelKeys[purchase.status])}
          tone={statusTones[purchase.status]}
        />
      </View>
      <AppText variant="caption" tone="muted">
        {datesOf(purchase)}
      </AppText>
      <AppText variant="caption" tone="muted">
        {includedClassesOf(purchase)}
      </AppText>
      {isActive ? (
        <View className="gap-1.5">
          <View className="flex-row">
            <ProgressBar
              ratio={
                purchase.classCount === 0 ? 0 : purchase.remainingClasses / purchase.classCount
              }
              tone="success"
              isThin
            />
          </View>
          <AppText variant="bodyStrong">
            {translate("student.classPacks.remaining", {
              remaining: purchase.remainingClasses,
              total: purchase.classCount,
            })}
          </AppText>
        </View>
      ) : null}
      {purchase.classes.length === 0 ? null : (
        <TextLink
          label={
            isShowingClasses
              ? translate("student.classPacks.hideClasses")
              : translateCount("student.classPacks.showClasses", purchase.classes.length)
          }
          trailingIcon={isShowingClasses ? "collapse" : "expand"}
          isExpanded={isShowingClasses}
          onPress={() => setIsShowingClasses(!isShowingClasses)}
        />
      )}
      {isShowingClasses ? (
        <View>
          {purchase.classes.map((packClass, index) => (
            <PackClassRow
              key={`${packClass.date}-${packClass.className}-${index}`}
              packClass={packClass}
              studentName={studentNameOf(packClass.studentId)}
              hasDivider={index < purchase.classes.length - 1}
            />
          ))}
        </View>
      ) : null}
    </OutlinedBlock>
  );
}
