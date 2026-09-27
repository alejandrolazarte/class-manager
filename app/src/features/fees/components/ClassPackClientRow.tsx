import { View } from "react-native";
import { summarizeNames } from "@/features/fees/summarizeNames";
import { ClassPackClient } from "@/features/fees/types";
import { translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { StatusPill } from "@/ui/StatusPill";

interface ClassPackClientRowProps {
  classPackClient: ClassPackClient;
  onPress: (classPackClient: ClassPackClient) => void;
}

export function ClassPackClientRow({ classPackClient, onPress }: ClassPackClientRowProps) {
  const owesClasses = classPackClient.unpaidClasses > 0;
  return (
    <Card
      onPress={() => onPress(classPackClient)}
      accessibilityLabel={classPackClient.clientFullName}
      className="flex-row items-center gap-3 rounded-[18px] px-3.5 py-[13px]"
    >
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong">{classPackClient.clientFullName}</AppText>
        <AppText variant="caption" tone="subtle">
          {summarizeNames(classPackClient.studentNames)}
        </AppText>
      </View>
      <StatusPill
        tone={owesClasses ? "danger" : "success"}
        label={
          owesClasses
            ? translateCount("classPacks.balance.unpaid", classPackClient.unpaidClasses)
            : translateCount("classPacks.balance.available", classPackClient.availableClasses)
        }
      />
    </Card>
  );
}
