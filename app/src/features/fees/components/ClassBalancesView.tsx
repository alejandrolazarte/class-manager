import { useRouter } from "expo-router";
import { ReactNode, useState } from "react";
import { FlatList, View } from "react-native";
import { ClassPackClientRow } from "@/features/fees/components/ClassPackClientRow";
import { monthOf } from "@/features/fees/months";
import { useMonthlyFees } from "@/features/fees/useFees";
import { useRefetchOnFocus } from "@/hooks/useRefetchOnFocus";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ChipGroup } from "@/ui/ChipGroup";
import { EmptyState } from "@/ui/EmptyState";
import { SkeletonList } from "@/ui/motion/SkeletonList";
import { AnimatedListItem } from "@/ui/motion/AnimatedListItem";

type ClassBalanceFilter = "all" | "owing";

interface ClassBalancesViewProps {
  header: ReactNode;
}

export function ClassBalancesView({ header }: ClassBalancesViewProps) {
  const router = useRouter();
  const [filter, setFilter] = useState<ClassBalanceFilter>("all");
  const {
    data: monthlyFees,
    isPending,
    isError,
    isRefetching,
    refetch,
  } = useMonthlyFees(monthOf());
  useRefetchOnFocus(refetch);
  const allClassPackClients = monthlyFees?.classPackClients ?? [];
  const classDebtors = allClassPackClients.filter(
    (classPackClient) => classPackClient.unpaidClasses > 0,
  );
  const classPackClients = filter === "all" ? allClassPackClients : classDebtors;
  const filterOptions = [
    { value: "all" as const, label: translate("classBalances.filter.all") },
    {
      value: "owing" as const,
      label: translate("classBalances.filter.owing", { count: classDebtors.length }),
      accessibilityLabel: translate("classBalances.filter.owingLabel"),
    },
  ];

  return (
    <FlatList
      data={isPending ? [] : classPackClients}
      keyExtractor={(classPackClient) => classPackClient.clientId}
      ListHeaderComponent={
        <View className="gap-3.5 pb-2">
          {header}
          <View className="gap-3.5 px-5">
            <ChipGroup options={filterOptions} selectedValue={filter} onChange={setFilter} />
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
      }
      renderItem={({ item: classPackClient, index }) => (
        <AnimatedListItem index={index} className="px-5">
          <ClassPackClientRow
            classPackClient={classPackClient}
            onPress={() => router.push(routes.clientDetail("students", classPackClient.clientId))}
          />
        </AnimatedListItem>
      )}
      ListEmptyComponent={
        isPending ? (
          <SkeletonList className="px-5" />
        ) : isError ? null : filter === "owing" ? (
          <EmptyState
            icon="celebration"
            iconTone="success"
            message={translate("classBalances.nobodyOwes")}
          />
        ) : (
          <EmptyState icon="classPacks" message={translate("classBalances.empty")} />
        )
      }
      refreshing={isRefetching}
      onRefresh={() => refetch()}
      contentContainerClassName="w-full max-w-2xl self-center pb-28"
    />
  );
}
