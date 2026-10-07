import { useState } from "react";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { Instructor } from "@/features/instructors/types";
import { useActiveInstructors } from "@/features/instructors/useActiveInstructors";
import { SubstituteInstructorPicker } from "@/features/sessions/components/SubstituteInstructorPicker";
import { sessionErrorCodes } from "@/features/sessions/sessionErrorCodes";
import { useAssignSubstitute } from "@/features/sessions/useSessionMutations";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ListRow } from "@/ui/ListRow";
import { PickerField } from "@/ui/PickerField";
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
  const [isPickerOpen, setIsPickerOpen] = useState(false);
  const [substitute, setSubstitute] = useState<Instructor | null>(null);
  const [failure, setFailure] = useState<SubstituteFailure | null>(null);
  const { data: instructors, isPending, isError } = useActiveInstructors({ enabled: isOpen });
  const offeredInstructors = (instructors ?? []).filter(
    (instructor) => instructor.id !== currentInstructorId,
  );

  const toggle = () => {
    setFailure(null);
    setSubstitute(null);
    setIsOpen(!isOpen);
  };

  const confirm = async () => {
    setFailure(null);
    if (substitute === null) {
      setFailure("pickInstructor");
      return;
    }
    try {
      await assignSubstituteMutation.mutateAsync(substitute.id);
      setIsOpen(false);
      setSubstitute(null);
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
          <PickerField
            label={translate("sessions.substitute.instructor")}
            chooseLabel={translate("sessions.substitute.chooseInstructor")}
            removeLabel={translate("sessions.substitute.removeInstructor")}
            picked={
              substitute === null
                ? null
                : { name: substitute.fullName, detail: substitute.email ?? "" }
            }
            onChoose={() => setIsPickerOpen(true)}
            onRemove={() => setSubstitute(null)}
          />
          <Button
            size="medium"
            label={translate("sessions.substitute.confirm")}
            onPress={confirm}
            isLoading={assignSubstituteMutation.isPending}
          />
        </View>
      ) : null}
      {isPickerOpen ? (
        <SubstituteInstructorPicker
          instructors={offeredInstructors}
          isPending={isPending}
          isError={isError}
          onPick={(instructor) => {
            setFailure(null);
            setSubstitute(instructor);
            setIsPickerOpen(false);
          }}
          onClose={() => setIsPickerOpen(false)}
        />
      ) : null}
    </View>
  );
}
