import { useQuery } from "@tanstack/react-query";
import { PropsWithChildren } from "react";
import { View } from "react-native";
import { CurrentMemberProvider } from "@/features/members/CurrentMemberProvider";
import { getCurrentMember } from "@/features/members/membersApi";
import { memberQueryKeys } from "@/features/members/memberQueryKeys";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { LoadingScreen } from "@/ui/LoadingScreen";

export function MemberProvider({ children }: PropsWithChildren) {
  const memberQuery = useQuery({
    queryKey: memberQueryKeys.current(),
    queryFn: getCurrentMember,
  });

  if (memberQuery.data) {
    return <CurrentMemberProvider member={memberQuery.data}>{children}</CurrentMemberProvider>;
  }
  if (memberQuery.isError) {
    return (
      <View className="flex-1 justify-center bg-background p-6">
        <Banner message={translate("members.loadError")}>
          <Button
            variant="secondary"
            label={translate("common.retry")}
            onPress={() => memberQuery.refetch()}
          />
        </Banner>
      </View>
    );
  }
  return <LoadingScreen />;
}
