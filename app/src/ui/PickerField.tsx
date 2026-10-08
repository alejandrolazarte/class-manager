import { Pressable, View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Icon, IconName } from "@/ui/Icon";
import { IconButton } from "@/ui/IconButton";

export interface PickedPerson {
  name: string;
  detail: string;
}

interface PickerChooseButtonProps {
  label: string;
  onPress: () => void;
  hint?: string;
  icon?: IconName;
}

interface PickedItemRowProps {
  name: string;
  detail?: string;
  removeLabel: string;
  onRemove?: () => void;
}

export function PickerChooseButton({
  label,
  onPress,
  hint,
  icon = "findPerson",
}: PickerChooseButtonProps) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={label}
      onPress={onPress}
      className="flex-row items-center gap-3 rounded-2xl border-[1.5px] border-border bg-surface px-3.5 py-3 active:bg-muted"
    >
      <Icon name={icon} tone="muted-foreground" />
      <View className="min-w-0 flex-1">
        <AppText variant="bodyStrong">{label}</AppText>
        {hint ? (
          <AppText variant="caption" tone="subtle">
            {hint}
          </AppText>
        ) : null}
      </View>
      <Icon name="next" tone="subtle-foreground" />
    </Pressable>
  );
}

export function PickedItemRow({ name, detail, removeLabel, onRemove }: PickedItemRowProps) {
  return (
    <View className="flex-row items-center gap-3 rounded-2xl border-[1.5px] border-primary bg-surface py-1.5 pl-3.5 pr-1.5">
      <Avatar name={name} size="small" />
      <View className="min-w-0 flex-1 py-1.5">
        <AppText variant="bodyStrong">{name}</AppText>
        {detail ? (
          <AppText variant="caption" tone="subtle">
            {detail}
          </AppText>
        ) : null}
      </View>
      {onRemove ? (
        <IconButton
          icon="close"
          tone="muted-foreground"
          accessibilityLabel={removeLabel}
          onPress={onRemove}
        />
      ) : null}
    </View>
  );
}

interface PickerFieldProps {
  label: string;
  chooseLabel: string;
  removeLabel: string;
  picked: PickedPerson | null;
  onChoose: () => void;
  onRemove: () => void;
  hint?: string;
  chooseIcon?: IconName;
}

export function PickerField({
  label,
  chooseLabel,
  removeLabel,
  picked,
  onChoose,
  onRemove,
  hint,
  chooseIcon,
}: PickerFieldProps) {
  return (
    <View className="gap-2">
      <AppText variant="label" tone="muted">
        {label}
      </AppText>
      {picked === null ? (
        <PickerChooseButton label={chooseLabel} hint={hint} icon={chooseIcon} onPress={onChoose} />
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
