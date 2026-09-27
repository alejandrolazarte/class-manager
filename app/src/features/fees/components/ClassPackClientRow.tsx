import { Pressable, View } from "react-native";
import { summarizeNames } from "@/features/fees/summarizeNames";
import { ClassPackClient } from "@/features/fees/types";
import { translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";

interface ClassPackClientRowProps {
  classPackClient: ClassPackClient;
  onPress: (classPackClient: ClassPackClient) => void;
}

export function ClassPackClientRow({ classPackClient, onPress }: ClassPackClientRowProps) {
  const owesClasses = classPackClient.unpaidClasses > 0;
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={classPackClient.clientFullName}
      onPress={() => onPress(classPackClient)}
      className="flex-row items-center gap-3 border-b border-border-subtle bg-surface px-4 py-3"
    >
      <View className="flex-1 gap-1">
        <AppText variant="bodyStrong">{classPackClient.clientFullName}</AppText>
        <AppText variant="caption" tone="muted">
          {summarizeNames(classPackClient.studentNames)}
        </AppText>
      </View>
      <AppText variant="label" tone={owesClasses ? "danger" : "success"}>
        {owesClasses
          ? translateCount("classPacks.balance.unpaid", classPackClient.unpaidClasses)
          : translateCount("classPacks.balance.available", classPackClient.availableClasses)}
      </AppText>
    </Pressable>
  );
}
