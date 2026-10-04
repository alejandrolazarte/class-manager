import { View } from "react-native";
import { StudentAppLevel } from "@/features/studentApp/types";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";

interface LevelPathProps {
  levels: readonly StudentAppLevel[];
  currentLevel: number;
}

function markerClassName(number: number, currentLevel: number): string {
  if (number < currentLevel) {
    return "bg-success";
  }
  return number === currentLevel ? "bg-primary" : "border-2 border-border bg-surface";
}

export function LevelPath({ levels, currentLevel }: LevelPathProps) {
  return (
    <Card className="px-4 py-3">
      {levels.map((level, index) => {
        const number = index + 1;
        const isLast = index === levels.length - 1;
        return (
          <View key={level.name} className="flex-row gap-3">
            <View className="w-8 items-center">
              <View
                className={`h-8 w-8 items-center justify-center rounded-full ${markerClassName(number, currentLevel)}`}
              >
                {number < currentLevel ? (
                  <Icon name="present" size="medium" tone="success-foreground" />
                ) : (
                  <AppText
                    variant="bodyStrong"
                    tone={number === currentLevel ? "onPrimary" : "subtle"}
                  >
                    {number}
                  </AppText>
                )}
              </View>
              {isLast ? null : (
                <View
                  className={`min-h-3 w-0.5 flex-1 ${number < currentLevel ? "bg-success" : "bg-border"}`}
                />
              )}
            </View>
            <View className={`min-w-0 flex-1 gap-0.5 pt-1 ${isLast ? "" : "pb-3"}`}>
              <AppText variant="bodyStrong" tone={number > currentLevel ? "subtle" : "default"}>
                {level.name}
              </AppText>
              <AppText variant="caption" tone="muted">
                {number === currentLevel
                  ? translate("student.progress.currentLevel")
                  : level.requiredClasses === 0
                    ? translate("student.progress.levelStart")
                    : translateCount("student.progress.levelFrom", level.requiredClasses)}
              </AppText>
            </View>
          </View>
        );
      })}
    </Card>
  );
}
