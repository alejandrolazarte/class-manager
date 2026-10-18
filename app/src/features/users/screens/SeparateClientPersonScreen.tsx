import { useClient } from "@/features/clients/useClient";
import { SeparatePersonScreen } from "@/features/users/screens/SeparatePersonScreen";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Screen, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SkeletonDetail } from "@/ui/motion/SkeletonDetail";

interface SeparateClientPersonScreenProps {
  clientId: string;
  studentId?: string;
}

export function SeparateClientPersonScreen({
  clientId,
  studentId,
}: SeparateClientPersonScreenProps) {
  const { data: client, isPending, isError, refetch } = useClient(clientId);
  if (isPending) {
    return (
      <Screen
        header={<ScreenHeader navigation="close" title={translate("people.separate.title")} />}
      >
        <SkeletonDetail className="w-full max-w-2xl self-center px-5 pt-4" />
      </Screen>
    );
  }
  const student = client?.students.find((candidate) => candidate.id === studentId);
  if (isError || client === undefined || (studentId !== undefined && student === undefined)) {
    return (
      <ScrollScreen
        header={<ScreenHeader navigation="close" title={translate("people.separate.title")} />}
      >
        <Banner message={translate("clients.detail.notFound")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => refetch()}
          />
        </Banner>
      </ScrollScreen>
    );
  }
  return student ? (
    <SeparatePersonScreen role="student" id={student.id} currentFullName={student.fullName} />
  ) : (
    <SeparatePersonScreen role="client" id={client.id} currentFullName={client.fullName} />
  );
}
