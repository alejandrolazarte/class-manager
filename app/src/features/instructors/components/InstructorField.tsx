import { useState } from "react";
import { View } from "react-native";
import { InstructorPicker } from "@/features/instructors/components/InstructorPicker";
import { Instructor } from "@/features/instructors/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { PickerField } from "@/ui/PickerField";

interface InstructorFieldProps {
  label: string;
  instructors: Instructor[];
  instructorId: string | null;
  onChange: (instructorId: string | null) => void;
  emptyMessage: string;
  testID: string;
  errorMessage?: string;
}

export function InstructorField({
  label,
  instructors,
  instructorId,
  onChange,
  emptyMessage,
  testID,
  errorMessage,
}: InstructorFieldProps) {
  const [isPickerOpen, setIsPickerOpen] = useState(false);
  const pickedInstructor = instructors.find((instructor) => instructor.id === instructorId);

  return (
    <View className="gap-2">
      <PickerField
        label={label}
        chooseLabel={translate("instructors.picker.choose")}
        removeLabel={translate("instructors.picker.remove")}
        picked={
          pickedInstructor === undefined
            ? null
            : { name: pickedInstructor.fullName, detail: pickedInstructor.email ?? "" }
        }
        onChoose={() => setIsPickerOpen(true)}
        onRemove={() => onChange(null)}
      />
      {errorMessage ? (
        <AppText variant="label" tone="danger" accessibilityRole="alert">
          {errorMessage}
        </AppText>
      ) : null}
      {isPickerOpen ? (
        <InstructorPicker
          testID={testID}
          instructors={instructors}
          isPending={false}
          isError={false}
          emptyMessage={emptyMessage}
          onPick={(instructor) => {
            onChange(instructor.id);
            setIsPickerOpen(false);
          }}
          onClose={() => setIsPickerOpen(false)}
        />
      ) : null}
    </View>
  );
}
