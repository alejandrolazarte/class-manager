import { ReactNode } from "react";
import { ScrollView, TextInput, View } from "react-native";
import { AppText, textVariantClassNames } from "@/ui/AppText";

export interface StockGridCell {
  key: string;
  value: number;
  accessibilityLabel: string;
}

export interface StockGridRow {
  key: string;
  label: string;
  leading?: ReactNode;
  cells: StockGridCell[];
}

interface StockGridProps {
  columnLabels: string[];
  rows: StockGridRow[];
  onChangeCell?: (cellKey: string, value: string) => void;
}

const cellClassName = "w-[60px] items-center";

export function StockGrid({ columnLabels, rows, onChangeCell }: StockGridProps) {
  return (
    <ScrollView horizontal showsHorizontalScrollIndicator={false}>
      <View>
        <View className="h-8 flex-row items-center">
          <View className="w-[110px]" />
          {columnLabels.map((columnLabel) => (
            <View key={columnLabel} className={cellClassName}>
              <AppText variant="label" tone="muted">
                {columnLabel}
              </AppText>
            </View>
          ))}
        </View>
        {rows.map((row) => (
          <View
            key={row.key}
            className="min-h-[52px] flex-row items-center border-t border-border-subtle"
          >
            <View className="w-[110px] flex-row items-center gap-2 pr-2">
              {row.leading}
              <AppText variant="bodyStrong" numberOfLines={1} className="shrink">
                {row.label}
              </AppText>
            </View>
            {row.cells.map((cell) => (
              <View key={cell.key} className={cellClassName}>
                {onChangeCell ? (
                  <TextInput
                    accessibilityLabel={cell.accessibilityLabel}
                    keyboardType="number-pad"
                    value={String(cell.value)}
                    onChangeText={(value) => onChangeCell(cell.key, value)}
                    className={`h-10 w-11 rounded-[10px] border border-border text-center ${textVariantClassNames.input} ${cell.value === 0 ? "bg-danger-soft text-danger-soft-foreground" : "bg-surface text-foreground"}`}
                  />
                ) : (
                  <AppText
                    variant="bodyStrong"
                    tone={cell.value === 0 ? "danger" : "default"}
                    accessibilityLabel={cell.accessibilityLabel}
                  >
                    {String(cell.value)}
                  </AppText>
                )}
              </View>
            ))}
          </View>
        ))}
        <View className="border-t border-border-subtle" />
      </View>
    </ScrollView>
  );
}
