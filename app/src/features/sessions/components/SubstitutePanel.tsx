import { useState } from "react";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { useActiveInstructors } from "@/features/instructors/useActiveInstructors";
import { sessionErrorCodes } from "@/features/sessions/sessionErrorCodes";
import { useAssignSubstitute } from "@/features/sessions/useSessionMutations";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { ListRow } from "@/ui/ListRow";
import { Spinner } from "@/ui/Spinner";
import { useToast } from "@/ui/ToastProvider";

interface SubstitutePanelProps {
  classGroupId: string;
  sessionDate: string;
  currentInstructorId: string;
}

type SubstituteFailure = "instructorBusy" | "pickInstructor" | "unexpected";

const failureMessages = {
  instructorBusy: "sessions.substitute.instructorBusy",
  pickInstructor: "sessions.substitute.pickInstructor",
  unexpected: "common.unexpectedError",
} as const;

export function SubstitutePanel({
  classGroupId,
  sessionDate,
  currentInstructorId,
}: SubstitutePanelProps) {
  const { showToast } = useToast();
  const assignSubstituteMutation = useAssignSubstitute(classGroupId, sessionDate);
  const [isOpen, setIsOpen] = useState(false);
  const [instructorId, setInstructorId] = useState<string | null>(null);
  const [failure, setFailure] = useState<SubstituteFailure | null>(null);
  const { data: instructors, isPending } = useActiveInstructors({ enabled: isOpen });
  const offeredInstructors = (instructors ?? []).filter(
    (instructor) => instructor.id !== currentInstructorId,
  );

  const toggle = () => {
    setFailure(null);
    setInstructorId(null);
    setIsOpen(!isOpen);
  };

  const confirm = async () => {
    setFailure(null);
    if (instructorId === null) {
      setFailure("pickInstructor");
      return;
    }
    try {
      await assignSubstituteMutation.mutateAsync(instructorId);
      setIsOpen(false);
      setInstructorId(null);
      showToast(translate("sessions.substitute.saved"));
    } catch (assignError) {
      setFailure(
        isApiError(assignError) && assignError.hasCode(sessionErrorCodes.instructorBusy)
          ? "instructorBusy"
          : "unexpected",
      );
    }
  };

  return (
    <View>
      <ListRow
        icon="substitute"
        label={translate("sessions.substitute.open")}
        isExpanded={isOpen}
        onPress={toggle}
      />
      {isOpen ? (
        <View className="gap-2.5 px-4 pb-4">
          {failure ? <Banner tone="warning" message={translate(failureMessages[failure])} /> : null}
          <AppText variant="label" tone="muted">
            {translate("sessions.substitute.instructor")}
          </AppText>
          {isPending ? (
            <Spinner />
          ) : offeredInstructors.length === 0 ? (
            <AppText variant="body" tone="muted">
              {translate("sessions.substitute.noInstructors")}
            </AppText>
          ) : (
            <View className="flex-row flex-wrap gap-2">
              {offeredInstructors.map((instructor) => (
                <Chip
                  key={instructor.id}
                  label={instructor.fullName}
                  isSelected={instructorId === instructor.id}
                  onPress={() => setInstructorId(instructor.id)}
                />
              ))}
            </View>
          )}
          <Button
            size="medium"
            label={translate("sessions.substitute.confirm")}
            onPress={confirm}
            isLoading={assignSubstituteMutation.isPending}
          />
        </View>
      ) : null}
    </View>
  );
}
