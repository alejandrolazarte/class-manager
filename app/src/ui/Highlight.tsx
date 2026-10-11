import { PropsWithChildren } from "react";
import { View } from "react-native";

export type HighlightTone = "primary" | "warning" | "success" | "danger" | "muted";

interface HighlightProps extends PropsWithChildren {
  tone: HighlightTone;
  isCompact?: boolean;
  testID?: string;
}

const toneClassNames: Record<HighlightTone, string> = {
  primary: "bg-primary-soft",
  warning: "bg-warning-soft",
  success: "bg-success-soft",
  danger: "bg-danger-soft",
  muted: "bg-muted",
};

export function Highlight({ tone, isCompact = false, testID, children }: HighlightProps) {
  return (
    <View
      testID={testID}
      className={`${isCompact ? "gap-1 rounded-xl px-3 py-2.5" : "gap-3 rounded-2xl p-4"} ${toneClassNames[tone]}`}
    >
      {children}
    </View>
  );
}
