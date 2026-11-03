import { useState } from "react";
import { View } from "react-native";
import { monthlyPlanHint } from "@/features/fees/planDescriptions";
import { ProductPhotoCarousel } from "@/features/studentApp/components/ProductPhotoCarousel";
import {
  PlanChoice,
  describePlanStudentStatus,
  planChoiceFor,
  planStudentStatus,
} from "@/features/studentApp/studentAppPlans";
import { StudentAppPlanStudent, StudentAppShopPlan } from "@/features/studentApp/types";
import { useChooseStudentAppPlan } from "@/features/studentApp/useStudentAppShop";
import { formatMoney } from "@/formatting/money";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
import { FullScreenPage } from "@/ui/FullScreenPage";
import { HairlineRow } from "@/ui/HairlineRow";
import { SectionTitle } from "@/ui/SectionTitle";
import { useToast } from "@/ui/ToastProvider";

interface PlanDetailPageProps {
  plan: StudentAppShopPlan;
  planStudents: readonly StudentAppPlanStudent[];
  currencyCode: string;
  canBuy: boolean;
  onClose: () => void;
}

interface PendingChoice extends PlanChoice {
  studentId: string;
}

export function PlanDetailPage({
  plan,
  planStudents,
  currencyCode,
  canBuy,
  onClose,
}: PlanDetailPageProps) {
  const { showToast } = useToast();
  const chooseStudentAppPlanMutation = useChooseStudentAppPlan();
  const [pendingChoice, setPendingChoice] = useState<PendingChoice | null>(null);
  const priceLabel = translate("student.shop.pricePerMonth", {
    price: formatMoney(plan.price, currencyCode),
  });

  const confirm = (choice: PendingChoice) => {
    chooseStudentAppPlanMutation.mutate(
      { studentId: choice.studentId, classPackId: choice.classPackId },
      {
        onSuccess: () => {
          setPendingChoice(null);
          showToast(
            translate(
              choice.isCancellation ? "student.shop.planCancelled" : "student.shop.planChosen",
            ),
          );
        },
        onError: () => {
          setPendingChoice(null);
          showToast(translate("common.unexpectedError"));
        },
      },
    );
  };

  return (
    <FullScreenPage title={plan.name} onClose={onClose}>
      <ProductPhotoCarousel
        images={plan.images}
        sizeClassName="aspect-square"
        placeholderIcon="classPacks"
        size="page"
      />
      <View className="gap-3.5 px-5 pb-6 pt-[18px]">
        <View className="gap-1">
          <AppText variant="display">{plan.name}</AppText>
          <AppText variant="headline" tone="primary">
            {priceLabel}
          </AppText>
        </View>
        {plan.description ? (
          <AppText variant="body" tone="muted">
            {plan.description}
          </AppText>
        ) : null}
        <AppText variant="label" tone="muted">
          {monthlyPlanHint(plan, currencyCode)}
        </AppText>
        {canBuy && planStudents.length > 0 ? (
          <View>
            <SectionTitle title={translate("student.shop.planFor")} />
            {planStudents.map((student) => {
              const choice = planChoiceFor(student, plan, priceLabel);
              return (
                <HairlineRow key={student.studentId}>
                  <View className="flex-1 gap-1">
                    <AppText variant="bodyStrong">{student.fullName}</AppText>
                    <AppText variant="caption" tone="subtle">
                      {describePlanStudentStatus(planStudentStatus(student, plan))}
                    </AppText>
                  </View>
                  <Button
                    size="small"
                    variant={choice.isCancellation ? "dangerOutline" : "primary"}
                    label={translate(
                      choice.isCancellation ? "student.shop.cancelPlan" : "student.shop.choosePlan",
                    )}
                    accessibilityLabel={`${translate(
                      choice.isCancellation ? "student.shop.cancelPlan" : "student.shop.choosePlan",
                    )} ${student.fullName}`}
                    onPress={() => setPendingChoice({ ...choice, studentId: student.studentId })}
                  />
                </HairlineRow>
              );
            })}
          </View>
        ) : null}
      </View>
      {pendingChoice === null ? null : (
        <ConfirmDialog
          title={pendingChoice.question}
          message={pendingChoice.consequence}
          confirmLabel={translate(
            pendingChoice.isCancellation ? "student.shop.cancelPlan" : "student.shop.choosePlan",
          )}
          onCancel={() => setPendingChoice(null)}
          onConfirm={() => confirm(pendingChoice)}
          isConfirming={chooseStudentAppPlanMutation.isPending}
        />
      )}
    </FullScreenPage>
  );
}
