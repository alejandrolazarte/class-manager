import { PropsWithChildren, ReactNode } from "react";
import { View } from "react-native";
import { AppText } from "@/ui/AppText";

interface HairlineSectionProps extends PropsWithChildren {
  title?: string;
  hint?: string;
  trailing?: ReactNode;
  isCompact?: boolean;
  testID?: string;
}

export function HairlineSection({
  title,
  hint,
  trailing,
  isCompact = false,
  testID,
  children,
}: HairlineSectionProps) {
  const hasHeader = title !== undefined || hint !== undefined || trailing !== undefined;
  return (
    <View
      testID={testID}
      className={`border-t border-border-subtle ${isCompact ? "gap-1.5 pt-3" : "gap-3 pt-4"}`}
    >
      {hasHeader ? (
        <View className="flex-row items-center gap-3">
          <View className="min-w-0 flex-1 gap-0.5">
            {title === undefined ? null : <AppText variant="bodyStrong">{title}</AppText>}
            {hint === undefined ? null : (
              <AppText variant="caption" tone="muted">
                {hint}
              </AppText>
            )}
          </View>
          {trailing}
        </View>
      ) : null}
      {children}
    </View>
  );
}
