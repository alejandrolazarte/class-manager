import { View } from "react-native";
import { AppText } from "@/ui/AppText";
import { GrowingFill } from "@/ui/motion/GrowingFill";

interface ProgressStepsProps {
  steps: readonly string[];
  currentStep: number;
}

export function ProgressSteps({ steps, currentStep }: ProgressStepsProps) {
  const lastStep = Math.max(1, steps.length - 1);
  return (
    <View className="relative">
      <View className="absolute left-[30px] right-[30px] top-[5px] h-[3px] overflow-hidden rounded-full bg-muted">
        <GrowingFill ratio={currentStep / lastStep} className="rounded-full bg-primary" />
      </View>
      <View className="flex-row justify-between">
        {steps.map((step, stepIndex) => {
          const isReached = stepIndex <= currentStep;
          return (
            <View key={step} className="w-[60px] items-center gap-1">
              <View
                testID={isReached ? "progress-step-reached" : "progress-step-pending"}
                className={`h-[13px] w-[13px] rounded-full border-2 ${isReached ? "border-primary bg-primary" : "border-border-strong bg-background"}`}
              />
              <AppText
                variant="tab"
                tone={isReached ? "default" : "subtle"}
                className={`text-center ${stepIndex === currentStep ? "font-strong" : ""}`}
              >
                {step}
              </AppText>
            </View>
          );
        })}
      </View>
    </View>
  );
}
