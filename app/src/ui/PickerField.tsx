import { Pressable, View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Icon } from "@/ui/Icon";
import { IconButton } from "@/ui/IconButton";

export interface PickedPerson {
  name: string;
  detail: string;
}

interface PickerFieldProps {
  label: string;
  chooseLabel: string;
  removeLabel: string;
  picked: PickedPerson | null;
  onChoose: () => void;
  onRemove: () => void;
  hint?: string;
}

export function PickerField({
  label,
  chooseLabel,
  removeLabel,
  picked,
  onChoose,
  onRemove,
  hint,
}: PickerFieldProps) {
  return (
    <View className="gap-2">
      <AppText variant="label" tone="muted">
        {label}
      </AppText>
      {picked === null ? (
        <Pressable
          accessibilityRole="button"
          accessibilityLabel={chooseLabel}
          onPress={onChoose}
          className="flex-row items-center gap-3 rounded-2xl border-[1.5px] border-border bg-surface px-3.5 py-3 active:bg-muted"
        >
          <Icon name="findPerson" tone="muted-foreground" />
          <View className="min-w-0 flex-1">
            <AppText variant="bodyStrong">{chooseLabel}</AppText>
            {hint ? (
              <AppText variant="caption" tone="subtle">
                {hint}
              </AppText>
            ) : null}
          </View>
          <Icon name="next" tone="subtle-foreground" />
        </Pressable>
      ) : (
        <View className="flex-row items-center gap-3 rounded-2xl border-[1.5px] border-primary bg-surface py-1.5 pl-3.5 pr-1.5">
          <Avatar name={picked.name} size="small" />
          <Pressable
            accessibilityRole="button"
            accessibilityLabel={chooseLabel}
            onPress={onChoose}
            className="min-w-0 flex-1 py-1.5"
          >
            <AppText variant="bodyStrong">{picked.name}</AppText>
            {picked.detail ? (
              <AppText variant="caption" tone="subtle">
                {picked.detail}
              </AppText>
            ) : null}
          </Pressable>
          <IconButton
            icon="close"
            tone="muted-foreground"
            accessibilityLabel={removeLabel}
            onPress={onRemove}
          />
        </View>
      )}
    </View>
  );
}
