import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Redirect, useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { isBackofficeSessionEnded } from "@/backoffice/api/backofficeHttpClient";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { operatorErrorMessage } from "@/backoffice/operators/operatorErrors";
import {
  AccessRequest,
  enableOperator,
  EnableOperatorRequest,
  getOperators,
  operatorsQueryKey,
  PlatformRole,
  platformRoles,
  PlatformTeamMember,
} from "@/backoffice/operators/operatorsApi";
import { hasTranslation, translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
import { Notice } from "@/ui/Notice";
import { OptionCard } from "@/ui/OptionCard";
import { RequiredFieldsLegend, withRequiredMark } from "@/ui/RequiredFieldsLegend";
import { ScreenFooter, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SkeletonScreen } from "@/ui/SkeletonScreen";
import { useToast } from "@/ui/ToastProvider";

const roleHintKeys: Record<PlatformRole, TranslationKey> = {
  PlatformOperator: "backoffice.enableOperator.roleHint.PlatformOperator",
  PlatformReader: "backoffice.enableOperator.roleHint.PlatformReader",
};

interface EnableOperatorScreenProps {
  accessRequestId: string;
}

function roleLabel(role: string): string {
  const key = `roles.${role}`;
  return hasTranslation(key) ? translate(key as TranslationKey) : role;
}

function memberHint(member: PlatformTeamMember): string {
  return [member.email, member.branchName, roleLabel(member.role)].filter(Boolean).join(" · ");
}

interface EnableOperatorFormProps {
  accessRequest: AccessRequest;
  teamMembers: PlatformTeamMember[];
}

function EnableOperatorForm({ accessRequest, teamMembers }: EnableOperatorFormProps) {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const [memberId, setMemberId] = useState<string | null>(null);
  const [role, setRole] = useState<PlatformRole | null>(null);
  const [isConfirming, setIsConfirming] = useState(false);
  const [submissionMessage, setSubmissionMessage] = useState<string | null>(null);
  const member = teamMembers.find((candidate) => candidate.memberId === memberId) ?? null;
  const enable = useMutation({
    mutationFn: (request: EnableOperatorRequest) => enableOperator(request),
  });
  const closeRoute = backofficeRoutes.operators;

  const confirm = async () => {
    if (member === null || role === null) {
      return;
    }
    setSubmissionMessage(null);
    try {
      await enable.mutateAsync({
        accessRequestId: accessRequest.id,
        memberId: member.memberId,
        role,
      });
      await queryClient.invalidateQueries({ queryKey: operatorsQueryKey });
      showToast(translate("backoffice.enableOperator.saved"));
      router.replace(closeRoute);
    } catch (error) {
      setSubmissionMessage(operatorErrorMessage(error));
    } finally {
      setIsConfirming(false);
    }
  };

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          title={translate("backoffice.enableOperator.title")}
          subtitle={accessRequest.email}
          navigation="close"
          onNavigate={() => router.replace(closeRoute)}
        />
      }
      footer={
        <ScreenFooter>
          <Button
            label={translate("backoffice.enableOperator.submit")}
            onPress={() => setIsConfirming(true)}
            disabled={member === null || role === null}
            isLoading={enable.isPending}
          />
        </ScreenFooter>
      }
    >
      <RequiredFieldsLegend />
      <Notice
        tone="warning"
        message={translate("backoffice.enableOperator.checkCode", { code: accessRequest.code })}
      />
      <View className="gap-2" accessibilityRole="radiogroup">
        <AppText variant="label" tone="muted">
          {withRequiredMark(translate("backoffice.enableOperator.member"))}
        </AppText>
        {teamMembers.length === 0 ? (
          <AppText tone="muted">{translate("backoffice.enableOperator.noTeamMembers")}</AppText>
        ) : (
          teamMembers.map((teamMember) => (
            <OptionCard
              key={teamMember.memberId}
              label={teamMember.fullName}
              hint={memberHint(teamMember)}
              isSelected={memberId === teamMember.memberId}
              onPress={() => setMemberId(teamMember.memberId)}
            />
          ))
        )}
      </View>
      <View className="gap-2" accessibilityRole="radiogroup">
        <AppText variant="label" tone="muted">
          {withRequiredMark(translate("backoffice.enableOperator.role"))}
        </AppText>
        {platformRoles.map((platformRole) => (
          <OptionCard
            key={platformRole}
            label={roleLabel(platformRole)}
            hint={translate(roleHintKeys[platformRole])}
            isSelected={role === platformRole}
            onPress={() => setRole(platformRole)}
          />
        ))}
      </View>
      {submissionMessage === null ? null : <Banner message={submissionMessage} />}
      {!isConfirming || member === null || role === null ? null : (
        <ConfirmDialog
          title={translate("backoffice.enableOperator.confirmTitle", {
            email: accessRequest.email,
            role: roleLabel(role),
          })}
          message={translate("backoffice.enableOperator.confirmMessage", {
            name: member.fullName,
            branch: member.branchName,
          })}
          confirmLabel={translate("backoffice.enableOperator.confirm")}
          onCancel={() => setIsConfirming(false)}
          onConfirm={confirm}
          isConfirming={enable.isPending}
        />
      )}
    </ScrollScreen>
  );
}

export function EnableOperatorScreen({ accessRequestId }: EnableOperatorScreenProps) {
  const router = useRouter();
  const operators = useQuery({ queryKey: operatorsQueryKey, queryFn: getOperators, retry: false });

  if (isBackofficeSessionEnded(operators.error)) {
    return <Redirect href={backofficeRoutes.signIn} />;
  }
  if (operators.data === undefined && !operators.isError) {
    return <SkeletonScreen />;
  }
  const accessRequest = operators.data?.accessRequests.find(
    (candidate) => candidate.id === accessRequestId,
  );
  if (operators.data === undefined || accessRequest === undefined) {
    return (
      <ScrollScreen
        header={
          <ScreenHeader
            title={translate("backoffice.enableOperator.title")}
            navigation="back"
            onNavigate={() => router.replace(backofficeRoutes.operators)}
          />
        }
      >
        <Banner
          message={
            operators.isError
              ? translate("common.unexpectedError")
              : translate("backoffice.enableOperator.notFound")
          }
        />
      </ScrollScreen>
    );
  }
  return (
    <EnableOperatorForm accessRequest={accessRequest} teamMembers={operators.data.teamMembers} />
  );
}
