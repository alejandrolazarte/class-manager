import { useRouter } from "expo-router";
import { useState } from "react";
import { ActivityIndicator, FlatList, Pressable, Text, View } from "react-native";
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
import { Chip } from "@/ui/Chip";

interface MonthlyFeesScreenProps {
  initialMonth?: string;
}

type FeeFilter = "debtors" | "all";

const debtorStatuses = new Set(["Unpaid", "Partial"]);

function MonthArrow({
  label,
  glyph,
  onPress,
}: {
  label: string;
  glyph: string;
  onPress: () => void;
}) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={label}
      onPress={onPress}
      className="px-4 py-2"
    >
      <Text className="text-2xl text-brand">{glyph}</Text>
    </Pressable>
  );
}

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
    <View className="flex-1 bg-gray-50">
      <View className="flex-row items-center justify-between bg-white py-2">
        <MonthArrow
          label={translate("fees.month.previous")}
          glyph="‹"
          onPress={() => setMonth(addMonths(month, -1))}
        />
        <Text className="text-lg font-semibold text-gray-900">{formatMonth(month)}</Text>
        <MonthArrow
          label={translate("fees.month.next")}
          glyph="›"
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
          <Text className="text-base font-medium text-gray-800">
            {translate("fees.month.summary", {
              paid: money(monthlyFees.totalPaid),
              due: money(monthlyFees.totalDue),
            })}
          </Text>
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
        <ActivityIndicator className="mt-6" />
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
              <Text className="p-6 text-center text-base text-gray-600">
                {translate(filter === "debtors" ? "fees.month.nobodyOwes" : "fees.month.empty")}
              </Text>
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
