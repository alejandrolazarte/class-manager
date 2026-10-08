import { useState } from "react";
import { View } from "react-native";
import {
  ClassGroupPicker,
  classGroupScheduleLine,
} from "@/features/classGroups/components/ClassGroupPicker";
import { ClassGroup } from "@/features/classGroups/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { PickedItemRow, PickerChooseButton } from "@/ui/PickerField";

interface ClassPackClassGroupsFieldProps {
  classGroups: ClassGroup[];
  classGroupIds: string[];
  onChange: (classGroupIds: string[]) => void;
}

export function ClassPackClassGroupsField({
  classGroups,
  classGroupIds,
  onChange,
}: ClassPackClassGroupsFieldProps) {
  const [isPickerOpen, setIsPickerOpen] = useState(false);
  const chosenClassGroups = classGroups.filter((classGroup) =>
    classGroupIds.includes(classGroup.id),
  );
  const canAddMore = chosenClassGroups.length < classGroups.length;

  return (
    <View className="gap-2">
      <AppText variant="label" tone="muted">
        {translate("classPacks.form.classGroups")}
      </AppText>
      {chosenClassGroups.map((classGroup) => (
        <PickedItemRow
          key={classGroup.id}
          name={classGroup.name}
          detail={classGroupScheduleLine(classGroup)}
          removeLabel={translate("classPacks.form.removeClassGroup", { name: classGroup.name })}
          onRemove={() =>
            onChange(classGroupIds.filter((classGroupId) => classGroupId !== classGroup.id))
          }
        />
      ))}
      {canAddMore ? (
        <PickerChooseButton
          icon="classes"
          label={translate(
            chosenClassGroups.length === 0
              ? "classPacks.form.chooseClassGroup"
              : "classPacks.form.addClassGroup",
          )}
          onPress={() => setIsPickerOpen(true)}
        />
      ) : null}
      <AppText variant="caption" tone="muted">
        {translate(
          classGroupIds.length === 0
            ? "classPacks.form.classGroupsNoneHint"
            : "classPacks.form.classGroupsHint",
        )}
      </AppText>
      {isPickerOpen ? (
        <ClassGroupPicker
          testID="class-pack-class-group-picker"
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
