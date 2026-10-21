import { View } from "react-native";
import { AppText } from "@/ui/AppText";
import { TonalBlock } from "@/ui/TonalBlock";

export interface LogDetail {
  label: string;
  value: string | number | null;
}

const jsonIndentation = 2;

function readablePayload(payload: string): string {
  try {
    return JSON.stringify(JSON.parse(payload), null, jsonIndentation);
  } catch {
    return payload;
  }
}

interface LogDetailsProps {
  details: readonly LogDetail[];
  payload?: string | null;
}

export function LogDetails({ details, payload }: LogDetailsProps) {
  return (
    <View className="gap-2 pb-3">
      {details
        .filter((detail) => detail.value !== null && detail.value !== "")
        .map((detail) => (
          <View key={detail.label} className="flex-row gap-2">
            <AppText variant="caption" tone="muted" className="w-24">
              {detail.label}
            </AppText>
            <AppText variant="caption" className="min-w-0 flex-1" selectable>
              {String(detail.value)}
            </AppText>
          </View>
        ))}
      {payload ? (
        <TonalBlock tone="muted" isCompact>
          <AppText variant="footnote" selectable>
            {readablePayload(payload)}
          </AppText>
        </TonalBlock>
      ) : null}
    </View>
  );
}
