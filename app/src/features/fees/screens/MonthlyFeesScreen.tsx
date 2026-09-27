import { useRouter } from "expo-router";
import { useState } from "react";
import { FlatList, View } from "react-native";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { ClientFeeRow } from "@/features/fees/components/ClientFeeRow";
import { formatMoney } from "@/features/fees/money";
import { addMonths, formatMonth, monthOf } from "@/features/fees/months";
import { ClientFee } from "@/features/fees/types";
import { useMonthlyFees } from "@/features/fees/useFees";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { StepArrow } from "@/ui/StepArrow";
import { Chip } from "@/ui/Chip";
import { AppText } from "@/ui/AppText";
import { Spinner } from "@/ui/Spinner";

interface MonthlyFeesScreenProps {
  initialMonth?: string;
}

type FeeFilter = "debtors" | "all";

const debtorStatuses = new Set(["Unpaid", "Partial"]);

export function MonthlyFeesScreen({ initialMonth }: MonthlyFeesScreenProps) {
  const router = useRouter();
  const business = useCurrentBusiness();
  const [month, setMonth] = useState(initialMonth ?? monthOf());
  const [filter, setFilter] = useState<FeeFilter>("debtors");
  const { data: monthlyFees, isPending, isError, isRefetching, refetch } = useMonthlyFees(month);
  const money = (amount: number) => formatMoney(amount, business.currencyCode);
  const clients = (monthlyFees?.clients ?? []).filter(
    (clientFee: ClientFee) => filter === "all" || debtorStatuses.has(clientFee.status),
  );

  return (
    <View className="flex-1 bg-background">
      <View className="flex-row items-center justify-between bg-surface py-2">
        <StepArrow
          direction="previous"
          label={translate("fees.month.previous")}
          onPress={() => setMonth(addMonths(month, -1))}
        />
        <AppText variant="heading">{formatMonth(month)}</AppText>
        <StepArrow
          direction="next"
          label={translate("fees.month.next")}
          onPress={() => setMonth(addMonths(month, 1))}
        />
      </View>
      <View className="gap-3 p-4">
        {business.defaultMonthlyFee === null ? (
          <Banner tone="warning" message={translate("fees.month.noDefaultFee")}>
            <Button
              variant="secondary"
              label={translate("fees.month.setDefaultFee")}
              onPress={() => router.push(routes.defaultMonthlyFee)}
            />
          </Banner>
        ) : null}
        {monthlyFees ? (
          <AppText variant="link">
            {translate("fees.month.summary", {
              paid: money(monthlyFees.totalPaid),
              due: money(monthlyFees.totalDue),
            })}
          </AppText>
        ) : null}
        <View className="flex-row gap-2">
          <Chip
            label={translate("fees.month.filterDebtors")}
            isSelected={filter === "debtors"}
            onPress={() => setFilter("debtors")}
          />
          <Chip
            label={translate("fees.month.filterAll")}
            isSelected={filter === "all"}
            onPress={() => setFilter("all")}
          />
        </View>
      </View>
      {isError ? (
        <View className="px-4">
          <Banner message={translate("common.unexpectedError")}>
            <Button
              variant="secondary"
              label={translate("common.retry")}
              onPress={() => refetch()}
            />
          </Banner>
        </View>
      ) : null}
      {isPending ? (
        <Spinner className="mt-6" />
      ) : (
        <FlatList
          data={clients}
          keyExtractor={(clientFee) => clientFee.clientId}
          renderItem={({ item: clientFee }) => (
            <ClientFeeRow
              clientFee={clientFee}
              onPress={() => router.push(routes.recordPayment(clientFee.clientId, month))}
            />
          )}
          ListEmptyComponent={
            isError ? null : (
              <AppText variant="body" tone="muted" className="p-6 text-center">
                {translate(filter === "debtors" ? "fees.month.nobodyOwes" : "fees.month.empty")}
              </AppText>
            )
          }
          refreshing={isRefetching}
          onRefresh={() => refetch()}
          contentContainerClassName="pb-12"
        />
      )}
    </View>
  );
}
