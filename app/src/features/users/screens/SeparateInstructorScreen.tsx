import { useInstructorsIncludingInactive } from "@/features/instructors/useInstructorsIncludingInactive";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { SeparatePersonScreen } from "@/features/users/screens/SeparatePersonScreen";
import { translate } from "@/i18n/translate";

interface SeparateInstructorScreenProps {
  instructorId: string;
}

export function SeparateInstructorScreen({ instructorId }: SeparateInstructorScreenProps) {
  const instructorsQuery = useInstructorsIncludingInactive();
  const instructor = instructorsQuery.data?.find((candidate) => candidate.id === instructorId);
  if (instructor === undefined) {
    return (
      <SettingsItemState
        isPending={instructorsQuery.isPending}
        isError={instructorsQuery.isError}
        notFoundMessage={translate("instructors.form.notFound")}
        onRetry={() => instructorsQuery.refetch()}
      />
    );
  }
  return (
    <SeparatePersonScreen
      role="instructor"
      id={instructor.id}
      currentFullName={instructor.fullName}
    />
  );
}
