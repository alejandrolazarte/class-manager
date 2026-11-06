import { View } from "react-native";
import { onlinePaymentStatusLabel } from "@/features/onlinePayments/onlinePaymentLabels";
import { OnlinePaymentAccount } from "@/features/onlinePayments/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { OutlinedBlock } from "@/ui/OutlinedBlock";
import { SectionTitle } from "@/ui/SectionTitle";

interface BrandAccountListProps {
  accounts: OnlinePaymentAccount[];
  onChoose: (account: OnlinePaymentAccount) => void;
  choosingAccountId: string | null;
}

export function BrandAccountList({ accounts, onChoose, choosingAccountId }: BrandAccountListProps) {
  return (
    <View className="gap-3">
      <SectionTitle title={translate("onlinePayments.brandAccounts")} />
      <AppText variant="body" tone="muted">
        {translate("onlinePayments.brandAccountsHint")}
      </AppText>
      {accounts.map((account) => {
        const branches =
          account.branchNames.length > 0
            ? account.branchNames.join(", ")
            : translate("onlinePayments.unusedAccount");
        return (
          <OutlinedBlock key={account.id}>
            <View className="flex-row items-center justify-between gap-3">
              <View className="flex-1">
                <AppText variant="bodyStrong">{branches}</AppText>
                <AppText variant="body" tone="muted">
                  {onlinePaymentStatusLabel(account.status)}
                </AppText>
              </View>
              <Button
                variant="secondary"
                size="small"
                label={translate("onlinePayments.useAccount")}
                accessibilityLabel={translate("onlinePayments.useAccountNamed", { branches })}
                isLoading={choosingAccountId === account.id}
                onPress={() => onChoose(account)}
              />
            </View>
          </OutlinedBlock>
        );
      })}
    </View>
  );
}
