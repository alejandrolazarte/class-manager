import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Redirect, useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { isBackofficeSessionEnded } from "@/backoffice/api/backofficeHttpClient";
import { formatDateTimeAsDate } from "@/backoffice/backofficeFormatting";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { operatorErrorMessage } from "@/backoffice/operators/operatorErrors";
import {
  BackofficeOperator,
  disableOperator,
  getOperators,
  operatorsQueryKey,
} from "@/backoffice/operators/operatorsApi";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
import { HairlineSection } from "@/ui/HairlineSection";
import { ListDivider, ListRow } from "@/ui/ListRow";
import { SkeletonList } from "@/ui/motion/SkeletonList";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { useToast } from "@/ui/ToastProvider";

function operatorName(operator: BackofficeOperator): string {
  return operator.fullName ?? operator.email;
}

function operatorDetail(operator: BackofficeOperator): string {
  const role =
    operator.role === null
      ? translate("backoffice.operators.withoutPlatformRole")
      : [translate(`roles.${operator.role}` as TranslationKey), operator.branchName]
          .filter(Boolean)
          .join(" · ");
  return `${operator.email} · ${role}`;
}

function useOperatorDisabling(onSettled: () => void) {
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  return useMutation({
    mutationFn: (operator: BackofficeOperator) => disableOperator(operator.loginId),
    onSuccess: () => showToast(translate("backoffice.operators.disabled")),
    onError: (error) => showToast(operatorErrorMessage(error)),
    onSettled: async () => {
      onSettled();
      await queryClient.invalidateQueries({ queryKey: operatorsQueryKey });
    },
  });
}

export function OperatorsScreen() {
  const router = useRouter();
  const [operatorToDisable, setOperatorToDisable] = useState<BackofficeOperator | null>(null);
  const operators = useQuery({ queryKey: operatorsQueryKey, queryFn: getOperators, retry: false });
  const disable = useOperatorDisabling(() => setOperatorToDisable(null));

  if (isBackofficeSessionEnded(operators.error)) {
    return <Redirect href={backofficeRoutes.signIn} />;
  }

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          eyebrow={translate("backoffice.signIn.title")}
          title={translate("backoffice.tabs.operators")}
        />
      }
    >
      {operators.isError ? (
        <Banner message={translate("common.unexpectedError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => operators.refetch()}
          />
        </Banner>
      ) : null}
      {operators.isPending ? <SkeletonList /> : null}
      {operators.data === undefined ? null : (
        <>
          <HairlineSection title={translate("backoffice.operators.accessRequestsTitle")} isCompact>
            {operators.data.accessRequests.length === 0 ? (
              <AppText tone="muted">{translate("backoffice.operators.noAccessRequests")}</AppText>
            ) : (
              operators.data.accessRequests.map((accessRequest, index) => (
                <View key={accessRequest.id}>
                  {index === 0 ? null : <ListDivider />}
                  <ListRow
                    label={accessRequest.email}
                    detail={translate("backoffice.operators.accessRequestDetail", {
                      code: accessRequest.code,
                      date: formatDateTimeAsDate(accessRequest.requestedAt),
                    })}
                    trailing={
                      <Button
                        variant="secondary"
                        size="small"
                        label={translate("backoffice.operators.enable")}
                        accessibilityLabel={translate("backoffice.operators.enableOf", {
                          email: accessRequest.email,
                        })}
                        onPress={() =>
                          router.push(backofficeRoutes.enableOperator(accessRequest.id))
                        }
                      />
                    }
                  />
                </View>
              ))
            )}
          </HairlineSection>
          <HairlineSection title={translate("backoffice.operators.enabledTitle")} isCompact>
            {operators.data.operators.length === 0 ? (
              <AppText tone="muted">{translate("backoffice.operators.noOperators")}</AppText>
            ) : (
              operators.data.operators.map((operator, index) => (
                <View key={operator.loginId}>
                  {index === 0 ? null : <ListDivider />}
                  <ListRow
                    label={operatorName(operator)}
                    detail={operatorDetail(operator)}
                    trailing={
                      operator.isCurrentOperator ? (
                        <AppText variant="caption" tone="subtle">
                          {translate("backoffice.operators.you")}
                        </AppText>
                      ) : (
                        <Button
                          variant="dangerGhost"
                          size="small"
                          label={translate("backoffice.operators.disable")}
                          accessibilityLabel={translate("backoffice.operators.disableOf", {
                            name: operatorName(operator),
                          })}
                          onPress={() => setOperatorToDisable(operator)}
                        />
                      )
                    }
                  />
                </View>
              ))
            )}
          </HairlineSection>
        </>
      )}
      {operatorToDisable === null ? null : (
        <ConfirmDialog
          title={translate("backoffice.operators.disableConfirmTitle", {
            name: operatorName(operatorToDisable),
          })}
          message={translate("backoffice.operators.disableConfirmMessage")}
          confirmLabel={translate("backoffice.operators.disable")}
          onCancel={() => setOperatorToDisable(null)}
          onConfirm={() => disable.mutate(operatorToDisable)}
          isConfirming={disable.isPending}
        />
      )}
    </ScrollScreen>
  );
}
