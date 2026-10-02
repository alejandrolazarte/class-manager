import { useRouter } from "expo-router";
import { ReactNode, useState } from "react";
import { FlatList, View } from "react-native";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { ClassPackClientRow } from "@/features/fees/components/ClassPackClientRow";
import { ClientFeeRow } from "@/features/fees/components/ClientFeeRow";
import { MonthlySummaryCard } from "@/features/fees/components/MonthlySummaryCard";
import { formatMoney } from "@/features/fees/money";
import { addMonths, formatMonth, monthOf } from "@/features/fees/months";
import { ClientFee } from "@/features/fees/types";
import { useMonthlyFees } from "@/features/fees/useFees";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { EmptyState } from "@/ui/EmptyState";
import { Screen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { SegmentedControl } from "@/ui/SegmentedControl";
import { Spinner } from "@/ui/Spinner";
import { StepArrow } from "@/ui/StepArrow";

interface MonthlyFeesScreenProps {
  initialMonth?: string;
  viewSwitcher?: ReactNode;
}

type FeeFilter = "debtors" | "all";

const debtorStatuses = new Set(["Unpaid", "Partial"]);

export function MonthlyFeesScreen({ initialMonth, viewSwitcher }: MonthlyFeesScreenProps) {
  const router = useRouter();
  const business = useCurrentBusiness();
  const canViewEveryPayment = useCan(permissions.paymentsViewAll);
  const [month, setMonth] = useState(initialMonth ?? monthOf());
  const [filter, setFilter] = useState<FeeFilter>("debtors");
  const { data: monthlyFees, isPending, isError, isRefetching, refetch } = useMonthlyFees(month);
  const money = (amount: number) => formatMoney(amount, business.currencyCode);
  const allClients = monthlyFees?.clients ?? [];
  const debtors = allClients.filter((clientFee: ClientFee) => debtorStatuses.has(clientFee.status));
  const clients = [...(filter === "all" ? allClients : debtors)].sort(
    (first, second) => second.balance - first.balance,
  );
  const classPackClients = (monthlyFees?.classPackClients ?? []).filter(
    (classPackClient) => filter === "all" || classPackClient.unpaidClasses > 0,
  );
  const filterOptions = [
    { value: "debtors" as const, key: "fees.month.filterDebtors" as const, count: debtors.length },
    { value: "all" as const, key: "fees.month.filterAll" as const, count: allClients.length },
  ].map(({ value, key, count }) => ({
    value,
    accessibilityLabel: translate(key),
    label: monthlyFees
      ? translate("fees.month.filterWithCount", { label: translate(key), count })
      : translate(key),
  }));

  const header = (
    <View className="gap-4 pb-3">
      <ScreenHeader
        eyebrow={translate(canViewEveryPayment ? "fees.month.title" : "fees.month.ownTitle")}
        title={formatMonth(month)}
        accessory={
          <View className="flex-row gap-1">
            <StepArrow
              direction="previous"
              label={translate("fees.month.previous")}
              onPress={() => setMonth(addMonths(month, -1))}
            />
            <StepArrow
              direction="next"
              label={translate("fees.month.next")}
              onPress={() => setMonth(addMonths(month, 1))}
            />
          </View>
        }
      />
      <View className="gap-4 px-5">
        {viewSwitcher}
        {business.defaultMonthlyFee === null ? (
          <Banner tone="warning" message={translate("fees.month.noDefaultFee")}>
            <Button
              variant="outline"
              size="medium"
              label={translate("fees.month.setDefaultFee")}
              onPress={() => router.push(routes.feesDefaultMonthlyFee)}
            />
          </Banner>
        ) : null}
        {monthlyFees ? (
          <MonthlySummaryCard
            monthlyFees={monthlyFees}
            debtorCount={debtors.length}
            owedAmount={debtors.reduce((total, debtor) => total + debtor.balance, 0)}
            money={money}
          />
        ) : null}
        {monthlyFees && monthlyFees.classPackSales > 0 ? (
          <AppText variant="label" tone="muted">
            {translate("fees.month.classPackSales", {
              amount: money(monthlyFees.classPackSales),
            })}
          </AppText>
        ) : null}
        <SegmentedControl options={filterOptions} selectedValue={filter} onChange={setFilter} />
        {isError ? (
          <Banner message={translate("common.unexpectedError")}>
            <Button
              variant="secondary"
              size="medium"
              label={translate("common.retry")}
              onPress={() => refetch()}
            />
          </Banner>
        ) : null}
      </View>
    </View>
  );

  return (
    <Screen>
      {isPending ? (
        <View>
          {header}
          <Spinner className="mt-6" />
        </View>
      ) : (
        <FlatList
          data={clients}
          keyExtractor={(clientFee) => clientFee.clientId}
          ListHeaderComponent={header}
          renderItem={({ item: clientFee }) => (
            <View className="px-5 pb-2.5">
              <ClientFeeRow
                clientFee={clientFee}
                onPress={() => router.push(routes.recordPayment("fees", clientFee.clientId, month))}
              />
            </View>
          )}
          ListEmptyComponent={
            isError || classPackClients.length > 0 ? null : filter === "debtors" ? (
              <EmptyState
                icon="celebration"
                iconTone="success"
                message={translate("fees.month.nobodyOwes")}
              />
            ) : (
              <EmptyState message={translate("fees.month.empty")} />
            )
          }
          ListFooterComponent={
            classPackClients.length > 0 ? (
              <View className="gap-2.5 px-5 pt-4">
                <SectionTitle title={translate("fees.month.classPacksTitle")} isOverline />
                {classPackClients.map((classPackClient) => (
                  <ClassPackClientRow
                    key={classPackClient.clientId}
                    classPackClient={classPackClient}
                    onPress={() =>
                      router.push(routes.clientDetail("fees", classPackClient.clientId))
                    }
                  />
                ))}
              </View>
            ) : null
          }
          refreshing={isRefetching}
          onRefresh={() => refetch()}
          contentContainerClassName="w-full max-w-2xl self-center pb-8"
        />
      )}
    </Screen>
  );
}
