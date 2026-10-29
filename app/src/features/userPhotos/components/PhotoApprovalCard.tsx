import { useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { View } from "react-native";
import { firstNameOf } from "@/features/studentApp/studentAppSchedule";
import { PhotoApproval } from "@/features/userPhotos/types";
import { approvePhoto, refusePhoto } from "@/features/userPhotos/userPhotoApi";
import { userPhotoQueryKeys } from "@/features/userPhotos/userPhotoQueryKeys";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
import { TonalBlock } from "@/ui/TonalBlock";
import { useToast } from "@/ui/ToastProvider";

type GuardianAnswer = "approved" | "refused";

interface PhotoApprovalCardProps {
  approval: PhotoApproval;
}

export function PhotoApprovalCard({ approval }: PhotoApprovalCardProps) {
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const [pendingAnswer, setPendingAnswer] = useState<GuardianAnswer | null>(null);
  const [isConfirmingRefusal, setIsConfirmingRefusal] = useState(false);
  const [hasFailed, setHasFailed] = useState(false);
  const firstName = firstNameOf(approval.studentFullName);

  const respond = async (answer: GuardianAnswer) => {
    setHasFailed(false);
    setPendingAnswer(answer);
    try {
      await (answer === "approved" ? approvePhoto(approval.id) : refusePhoto(approval.id));
      setIsConfirmingRefusal(false);
      showToast(
        translate(answer === "approved" ? "photoApproval.approved" : "photoApproval.refused", {
          name: firstName,
        }),
      );
      await queryClient.invalidateQueries({ queryKey: userPhotoQueryKeys.approvals });
    } catch {
      setIsConfirmingRefusal(false);
      setHasFailed(true);
    } finally {
      setPendingAnswer(null);
    }
  };

  return (
    <TonalBlock tone="warning">
      <View className="flex-row items-center gap-3">
        <Avatar name={approval.studentFullName} photoUrl={approval.photoUrl} size="large" />
        <View className="min-w-0 flex-1 gap-0.5">
          <AppText variant="bodyStrong">
            {translate("photoApproval.cardTitle", { name: firstName })}
          </AppText>
          <AppText variant="caption" tone="warningSoft">
            {translate("photoApproval.details")}
          </AppText>
        </View>
      </View>
      {hasFailed ? <Banner message={translate("common.unexpectedError")} /> : null}
      <View className="flex-row gap-2">
        <View className="flex-1">
          <Button
            size="medium"
            label={translate("photoApproval.approve")}
            onPress={() => void respond("approved")}
            disabled={pendingAnswer === "refused"}
            isLoading={pendingAnswer === "approved"}
          />
        </View>
        <View className="flex-1">
          <Button
            size="medium"
            variant="secondary"
            label={translate("photoApproval.refuse")}
            onPress={() => setIsConfirmingRefusal(true)}
            disabled={pendingAnswer === "approved"}
          />
        </View>
      </View>
      {isConfirmingRefusal ? (
        <ConfirmDialog
          title={translate("photoApproval.refuseQuestion", { name: firstName })}
          message={translate("photoApproval.refuseMessage")}
          confirmLabel={translate("photoApproval.confirmRefuse")}
          onCancel={() => setIsConfirmingRefusal(false)}
          onConfirm={() => void respond("refused")}
          isConfirming={pendingAnswer === "refused"}
        />
      ) : null}
    </TonalBlock>
  );
}
