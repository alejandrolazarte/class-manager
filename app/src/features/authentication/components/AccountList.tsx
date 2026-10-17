import { View } from "react-native";
import { Account } from "@/features/authentication/types";
import { accountKeyOf } from "@/features/authentication/useOpenAccount";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { HairlineRow } from "@/ui/HairlineRow";
import { Icon } from "@/ui/Icon";
import { IconCircle } from "@/ui/IconCircle";
import { Spinner } from "@/ui/Spinner";
import { SkeletonList } from "@/ui/motion/SkeletonList";
import { StatusPill } from "@/ui/StatusPill";

interface AccountListProps {
  accounts: Account[] | undefined;
  isPending: boolean;
  switchingKey: string | null;
  hasFailed: boolean;
  onOpen: (account: Account) => void;
}

function accountLabel(account: Account): string {
  return translate(account.kind === "student" ? "accounts.asStudent" : "accounts.asTeam", {
    name: account.businessName,
  });
}

export function AccountList({
  accounts,
  isPending,
  switchingKey,
  hasFailed,
  onOpen,
}: AccountListProps) {
  return (
    <>
      {hasFailed ? <Banner message={translate("accounts.switchFailed")} /> : null}
      {isPending ? <SkeletonList /> : null}
      <View>
        {(accounts ?? []).map((account) => {
          const key = accountKeyOf(account);
          return (
            <HairlineRow
              key={key}
              onPress={() => onOpen(account)}
              accessibilityLabel={accountLabel(account)}
            >
              <IconCircle icon={account.kind === "student" ? "home" : "business"} tone="primary" />
              <View className="min-w-0 flex-1 gap-0.5">
                <AppText variant="heading">{account.businessName}</AppText>
                <AppText variant="caption" tone="subtle">
                  {translate(
                    account.kind === "student" ? "accounts.studentHint" : "accounts.teamHint",
                  )}
                </AppText>
              </View>
              {switchingKey === key ? (
                <Spinner />
              ) : account.isCurrent ? (
                <StatusPill isSmall label={translate("accounts.current")} tone="primary" />
              ) : (
                <Icon name="next" size="medium" tone="subtle-foreground" />
              )}
            </HairlineRow>
          );
        })}
      </View>
    </>
  );
}
