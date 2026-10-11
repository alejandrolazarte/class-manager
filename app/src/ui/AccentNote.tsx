import { PropsWithChildren } from "react";
import { View } from "react-native";

type AccentNoteTone = "primary" | "warning";

interface AccentNoteProps extends PropsWithChildren {
  tone: AccentNoteTone;
}

const barClassNames: Record<AccentNoteTone, string> = {
  primary: "bg-primary-soft",
  warning: "bg-warning-soft",
};

export function AccentNote({ tone, children }: AccentNoteProps) {
  return (
    <View className="flex-row gap-3">
      <View testID="accent-note-bar" className={`w-[3px] rounded-full ${barClassNames[tone]}`} />
      <View className="min-w-0 flex-1 gap-0.5">{children}</View>
    </View>
  );
}
