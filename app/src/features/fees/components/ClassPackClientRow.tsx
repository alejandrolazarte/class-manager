import { View } from "react-native";
import { summarizeNames } from "@/features/fees/summarizeNames";
import { ClassPackClient } from "@/features/fees/types";
import { translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";
import { StatusText } from "@/ui/StatusText";

interface ClassPackClientRowProps {
  classPackClient: ClassPackClient;
  onPress: (classPackClient: ClassPackClient) => void;
}

export function ClassPackClientRow({ classPackClient, onPress }: ClassPackClientRowProps) {
  const owesClasses = classPackClient.unpaidClasses > 0;
  return (
    <HairlineRow
      onPress={() => onPress(classPackClient)}
      accessibilityLabel={classPackClient.clientFullName}
    >
      <View className="min-w-0 flex-1">
        <AppText variant="bodyStrong" numberOfLines={1}>
          {classPackClient.clientFullName}
        </AppText>
        <AppText variant="caption" tone="subtle">
          {summarizeNames(classPackClient.studentNames)}
        </AppText>
      </View>
      <StatusText
        tone={owesClasses ? "danger" : "success"}
        label={
          owesClasses
            ? translateCount("classPacks.balance.unpaid", classPackClient.unpaidClasses)
            : translateCount("classPacks.balance.available", classPackClient.availableClasses)
        }
      />
    </HairlineRow>
  );
}
