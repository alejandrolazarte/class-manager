import { View } from "react-native";
import { StudentAppLevel } from "@/features/studentApp/types";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";
import { StatusPill } from "@/ui/StatusPill";

interface LevelPathProps {
  levels: readonly StudentAppLevel[];
  currentLevel: number;
}

function markerClassName(number: number, currentLevel: number): string {
  if (number < currentLevel) {
    return "bg-primary";
  }
  return number === currentLevel
    ? "border-4 border-primary-soft bg-primary"
    : "border-2 border-border-strong bg-background";
}

function lineClassName(isReached: boolean): string {
  return `w-0.5 flex-1 ${isReached ? "bg-primary" : "bg-border"}`;
}

export function LevelPath({ levels, currentLevel }: LevelPathProps) {
  return (
    <View>
      {levels.map((level, index) => {
        const number = index + 1;
        const isFirst = index === 0;
        const isLast = index === levels.length - 1;
        const isCurrent = number === currentLevel;
        return (
          <View key={level.name} className="flex-row gap-3">
            <View className="w-7 items-center">
              <View className={isFirst ? "w-0.5 flex-1" : lineClassName(number <= currentLevel)} />
              <View
                className={`h-7 w-7 items-center justify-center rounded-full ${markerClassName(number, currentLevel)}`}
              >
                {number < currentLevel ? (
                  <Icon name="present" size="small" tone="primary-foreground" />
                ) : (
                  <AppText variant="badge" tone={isCurrent ? "onPrimary" : "subtle"}>
                    {number}
                  </AppText>
                )}
              </View>
              <View className={isLast ? "w-0.5 flex-1" : lineClassName(number < currentLevel)} />
            </View>
            <View className="min-w-0 flex-1 flex-row items-center gap-2 py-2.5">
              <View className="min-w-0 flex-1 gap-px">
                <AppText
                  variant={isCurrent ? "bodyStrong" : "body"}
                  tone={number > currentLevel ? "subtle" : "default"}
                >
                  {level.name}
                </AppText>
                {isCurrent ? null : (
                  <AppText variant="footnote" tone="muted">
                    {level.requiredClasses === 0
                      ? translate("student.progress.levelStart")
                      : translateCount("student.progress.levelFrom", level.requiredClasses)}
                  </AppText>
                )}
              </View>
              {isCurrent ? (
                <StatusPill
                  isSmall
                  label={translate("student.progress.currentLevel")}
                  tone="primary"
                />
              ) : null}
            </View>
          </View>
        );
      })}
    </View>
  );
}
