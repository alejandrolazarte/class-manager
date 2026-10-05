import { View } from "react-native";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { IconButton } from "@/ui/IconButton";
import { TextField } from "@/ui/TextField";

export interface LevelDraft {
  name: string;
  requiredClasses: string;
}

interface LevelRowProps {
  number: number;
  level: LevelDraft;
  isFirst: boolean;
  canRemove: boolean;
  onChange: (level: LevelDraft) => void;
  onRemove: () => void;
}

export function LevelRow({ number, level, isFirst, canRemove, onChange, onRemove }: LevelRowProps) {
  return (
    <View className="flex-row items-end gap-2">
      <View className="mb-3 h-8 w-8 items-center justify-center rounded-full bg-primary-soft">
        <AppText variant="bodyStrong" tone="primary">
          {number}
        </AppText>
      </View>
      <View className="min-w-0 flex-1">
        <TextField
          label={translate("achievements.settings.nameLabel")}
          isRequired
          accessibilityLabel={translate("achievements.settings.levelName", { number })}
          value={level.name}
          autoCapitalize="words"
          onChangeText={(name) => onChange({ ...level, name })}
        />
      </View>
      <View className="w-24">
        <TextField
          label={translate("achievements.settings.classesLabel")}
          isRequired
          accessibilityLabel={translate("achievements.settings.levelClasses", { number })}
          value={level.requiredClasses}
          keyboardType="number-pad"
          editable={!isFirst}
          onChangeText={(requiredClasses) => onChange({ ...level, requiredClasses })}
        />
      </View>
      <View className="mb-1">
        <IconButton
          icon="delete"
          accessibilityLabel={translate("achievements.settings.removeLevel", { number })}
          disabled={!canRemove}
          onPress={onRemove}
        />
      </View>
    </View>
  );
}
