import { View } from "react-native";
import { StudentAppLevel } from "@/features/studentApp/types";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";
import { LivePulse } from "@/ui/motion/LivePulse";
import { ProgressBar } from "@/ui/ProgressBar";
import { StatusPill } from "@/ui/StatusPill";

interface LevelPathProps {
  levels: readonly StudentAppLevel[];
  currentLevel: number;
  attendedClasses: number;
}

function markerClassName(number: number, currentLevel: number, isFinal: boolean): string {
  if (isFinal) {
    return number <= currentLevel
      ? "bg-warning"
      : "border-2 border-dashed border-warning bg-warning-soft";
  }
  if (number <= currentLevel) {
    return number === currentLevel ? "border-4 border-primary-soft bg-primary" : "bg-primary";
  }
  return "border-2 border-dashed border-border-strong bg-surface";
}

const fullPercentage = 100;

function connectorRatio(
  levels: readonly StudentAppLevel[],
  index: number,
  currentLevel: number,
  attendedClasses: number,
): number {
  const number = index + 1;
  if (number < currentLevel) {
    return 1;
  }
  const next = levels[index + 1];
  if (number > currentLevel || next === undefined) {
    return 0;
  }
  const span = next.requiredClasses - levels[index].requiredClasses;
  const ratio = span <= 0 ? 0 : (attendedClasses - levels[index].requiredClasses) / span;
  return Math.max(0, Math.min(1, ratio));
}

export function LevelPath({ levels, currentLevel, attendedClasses }: LevelPathProps) {
  const next = levels[currentLevel] ?? null;
  return (
    <View>
      {levels.map((level, index) => {
        const number = index + 1;
        const isFinal = index === levels.length - 1;
        const isCurrent = number === currentLevel;
        const isReached = number <= currentLevel;
        const fillRatio = connectorRatio(levels, index, currentLevel, attendedClasses);
        const marker = (
          <View
            className={`h-12 w-12 items-center justify-center rounded-full ${markerClassName(number, currentLevel, isFinal)}`}
          >
            {isFinal ? (
              <Icon
                name="militaryTech"
                size="medium"
                tone={isReached ? "primary-foreground" : "warning"}
              />
            ) : number < currentLevel ? (
              <Icon name="present" size="medium" tone="primary-foreground" />
            ) : (
              <AppText variant="heading" tone={isReached ? "onPrimary" : "subtle"}>
                {number}
              </AppText>
            )}
          </View>
        );
        return (
          <View key={`${number}-${level.name}`} className="flex-row gap-3.5">
            <View className="w-12 items-center">
              {isCurrent ? (
                <LivePulse ringClassName="rounded-full bg-primary">{marker}</LivePulse>
              ) : (
                marker
              )}
              {isFinal ? null : (
                <View className="my-1 min-h-6 w-1.5 flex-1 flex-row overflow-hidden rounded-full bg-muted">
                  <View
                    className="w-full self-start rounded-full bg-primary"
                    style={{ height: `${Math.round(fillRatio * fullPercentage)}%` }}
                  />
                </View>
              )}
            </View>
            <View className="min-w-0 flex-1 gap-1 pb-6 pt-1.5">
              <View className="flex-row flex-wrap items-center gap-2">
                <AppText
                  variant={isCurrent ? "heading" : "bodyStrong"}
                  tone={isReached ? "default" : "muted"}
                >
                  {level.name}
                </AppText>
                {isCurrent ? (
                  <StatusPill
                    isSmall
                    label={translate("student.progress.currentLevel")}
                    tone="primary"
                  />
                ) : null}
                {isFinal ? (
                  <StatusPill
                    isSmall
                    label={translate("student.levels.finalGoal")}
                    tone="warning"
                  />
                ) : null}
              </View>
              <AppText variant="footnote" tone="muted">
                {level.requiredClasses === 0
                  ? translate("student.progress.levelStart")
                  : translateCount("student.progress.levelFrom", level.requiredClasses)}
              </AppText>
              {number < currentLevel ? (
                <View className="flex-row items-center gap-1">
                  <Icon name="present" size="small" tone="success" />
                  <AppText variant="caption" tone="success">
                    {translate("student.levels.achieved")}
                  </AppText>
                </View>
              ) : null}
              {isCurrent && next !== null ? (
                <View className="mt-1 gap-1.5">
                  <View className="flex-row">
                    <ProgressBar ratio={fillRatio} />
                  </View>
                  <AppText variant="caption" tone="muted">
                    {translate("student.progress.classesOfNext", {
                      count: attendedClasses,
                      total: next.requiredClasses,
                    })}
                  </AppText>
                </View>
              ) : null}
              {number > currentLevel ? (
                <View className="flex-row items-center gap-1">
                  <Icon name="locked" size="small" tone="subtle-foreground" />
                  <AppText variant="caption" tone="muted">
                    {translateCount(
                      "student.levels.classesLeft",
                      Math.max(0, level.requiredClasses - attendedClasses),
                    )}
                  </AppText>
                </View>
              ) : null}
            </View>
          </View>
        );
      })}
    </View>
  );
}
