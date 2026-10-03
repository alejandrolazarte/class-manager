import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { useSession } from "@/features/authentication/useSession";
import { Branch } from "@/features/branches/types";
import { useBranches } from "@/features/branches/useBranches";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { memberRoleLabel } from "@/features/roles/roleChoices";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { FloatingActionButton } from "@/ui/FloatingActionButton";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { Spinner } from "@/ui/Spinner";
import { StatusPill } from "@/ui/StatusPill";
import { useToast } from "@/ui/ToastProvider";
import { TitleWithPills } from "@/ui/TitleWithPills";
import { LockedFeatureNotice } from "@/features/subscriptions/components/LockedFeatureNotice";
import { featureCodes } from "@/features/subscriptions/subscriptionCodes";
import { useAllowsAnother } from "@/features/subscriptions/useSubscription";

function branchRoleDescription(branch: Branch): string {
  if (branch.isBrandOwner) {
    return translate("branches.brandOwner");
  }
  if (branch.branchRole === "Custom") {
    return branch.customRoleName ?? memberRoleLabel(branch.branchRole, null, undefined);
  }
  return branch.branchRole === null ? "" : memberRoleLabel(branch.branchRole, null, undefined);
}

export function BranchesScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const { switchBranch } = useSession();
  const branchesQuery = useBranches();
  const isWithinBranchLimit = useAllowsAnother(
    featureCodes.branches,
    branchesQuery.data?.length ?? 0,
  );
  const hasBranchPermission = useCan(permissions.branchesCreate);
  const canCreateBranches = hasBranchPermission && isWithinBranchLimit;
  const [switchingBranchId, setSwitchingBranchId] = useState<string | null>(null);
  const [hasSwitchFailed, setHasSwitchFailed] = useState(false);

  const openBranch = async (branch: Branch) => {
    if (branch.isCurrent || switchingBranchId !== null) {
      return;
    }
    setHasSwitchFailed(false);
    setSwitchingBranchId(branch.businessId);
    try {
      await switchBranch(branch.businessId);
      showToast(translate("branches.switched", { name: branch.name }));
      router.replace(routes.today);
    } catch {
      setHasSwitchFailed(true);
    } finally {
      setSwitchingBranchId(null);
    }
  };

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="back" title={translate("branches.title")} />}
      hasFloatingAction={canCreateBranches}
      overlay={
        canCreateBranches ? (
          <FloatingActionButton
            label={translate("branches.new")}
            onPress={() => router.push(routes.newBranch)}
          />
        ) : undefined
      }
    >
      {hasBranchPermission && branchesQuery.data && !isWithinBranchLimit ? (
        <LockedFeatureNotice message={translate("subscriptions.locked.branches")} />
      ) : null}
      {hasSwitchFailed ? <Banner message={translate("branches.switchFailed")} /> : null}
      {branchesQuery.isPending ? <Spinner className="mt-6" /> : null}
      {branchesQuery.isError ? (
        <Banner message={translate("common.unexpectedError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => branchesQuery.refetch()}
          />
        </Banner>
      ) : null}
      {(branchesQuery.data ?? []).map((branch) => (
        <Card
          key={branch.businessId}
          onPress={branch.isCurrent ? undefined : () => openBranch(branch)}
          accessibilityLabel={branch.isCurrent ? undefined : branch.name}
          className="flex-row items-center gap-3 rounded-[18px] px-3.5 py-3"
        >
          <View className="min-w-0 flex-1 gap-0.5">
            <TitleWithPills title={branch.name}>
              {branch.isCurrent ? (
                <StatusPill label={translate("branches.current")} tone="primary" isSmall />
              ) : null}
            </TitleWithPills>
            <AppText variant="caption" tone="subtle">
              {branchRoleDescription(branch)}
            </AppText>
          </View>
          {switchingBranchId === branch.businessId ? <Spinner /> : null}
        </Card>
      ))}
    </ScrollScreen>
  );
}
