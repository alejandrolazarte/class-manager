import { PropsWithChildren } from "react";
import { View } from "react-native";

export type TonalBlockTone =
  "primary" | "warning" | "success" | "danger" | "muted" | "surface" | "inverse";

interface TonalBlockProps extends PropsWithChildren {
  tone: TonalBlockTone;
  isCompact?: boolean;
  testID?: string;
}

const toneClassNames: Record<TonalBlockTone, string> = {
  primary: "bg-primary-soft",
  warning: "bg-warning-soft",
  success: "bg-success-soft",
  danger: "bg-danger-soft",
  muted: "bg-muted",
  surface: "bg-surface",
  inverse: "bg-inverse",
};

export function TonalBlock({ tone, isCompact = false, testID, children }: TonalBlockProps) {
  return (
    <View
      testID={testID}
      className={`${isCompact ? "gap-1 rounded-xl px-3 py-2.5" : "gap-3 rounded-2xl p-4"} ${toneClassNames[tone]}`}
    >
      {children}
    </View>
  );
}
