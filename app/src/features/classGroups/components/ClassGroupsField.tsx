import { useState } from "react";
import { View } from "react-native";
import {
  ClassGroupPicker,
  classGroupScheduleLine,
} from "@/features/classGroups/components/ClassGroupPicker";
import { ClassGroup } from "@/features/classGroups/types";
import { AppText } from "@/ui/AppText";
import { PickedItemRow, PickerChooseButton } from "@/ui/PickerField";

interface ClassGroupsFieldProps {
  classGroups: ClassGroup[];
  classGroupIds: string[];
  onChange: (classGroupIds: string[]) => void;
  label?: string;
  chooseLabel: string;
  addLabel: string;
  removeLabel: (classGroupName: string) => string;
  hint: string;
  pickerTestID: string;
}

export function ClassGroupsField({
  classGroups,
  classGroupIds,
  onChange,
  label,
  chooseLabel,
  addLabel,
  removeLabel,
  hint,
  pickerTestID,
}: ClassGroupsFieldProps) {
  const [isPickerOpen, setIsPickerOpen] = useState(false);
  const chosenClassGroups = classGroups.filter((classGroup) =>
    classGroupIds.includes(classGroup.id),
  );
  const canAddMore = chosenClassGroups.length < classGroups.length;

  return (
    <View className="gap-2">
      {label === undefined ? null : (
        <AppText variant="label" tone="muted">
          {label}
        </AppText>
      )}
      {chosenClassGroups.map((classGroup) => (
        <PickedItemRow
          key={classGroup.id}
          name={classGroup.name}
          detail={classGroupScheduleLine(classGroup)}
          removeLabel={removeLabel(classGroup.name)}
          onRemove={() =>
            onChange(classGroupIds.filter((classGroupId) => classGroupId !== classGroup.id))
          }
        />
      ))}
      {canAddMore ? (
        <PickerChooseButton
          icon="classes"
          label={chosenClassGroups.length === 0 ? chooseLabel : addLabel}
          onPress={() => setIsPickerOpen(true)}
        />
      ) : null}
      <AppText variant="caption" tone="muted">
        {hint}
      </AppText>
      {isPickerOpen ? (
        <ClassGroupPicker
          testID={pickerTestID}
          classGroups={classGroups}
          excludedClassGroupIds={classGroupIds}
          onPick={(classGroup) => {
            onChange([...classGroupIds, classGroup.id]);
            setIsPickerOpen(false);
          }}
          onClose={() => setIsPickerOpen(false)}
        />
      ) : null}
    </View>
  );
}
