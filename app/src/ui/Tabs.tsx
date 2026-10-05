import { Pressable, View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";

export interface TabOption<TValue extends string> {
  value: TValue;
  label: string;
  icon?: IconName;
  accessibilityLabel?: string;
}

interface TabsProps<TValue extends string> {
  options: readonly TabOption<TValue>[];
  selectedValue: TValue;
  onChange: (value: TValue) => void;
}

export function Tabs<TValue extends string>({
  options,
  selectedValue,
  onChange,
}: TabsProps<TValue>) {
  return (
    <View accessibilityRole="tablist" className="flex-row border-b border-border">
      {options.map((option) => {
        const isSelected = option.value === selectedValue;
        return (
          <Pressable
            key={option.value}
            accessibilityRole="tab"
            accessibilityLabel={option.accessibilityLabel ?? option.label}
            accessibilityState={{ selected: isSelected }}
            onPress={() => onChange(option.value)}
            className={`-mb-px h-12 flex-1 flex-row items-center justify-center gap-1.5 border-b-[3px] px-2 ${isSelected ? "border-primary" : "border-transparent active:bg-muted"}`}
          >
            {option.icon ? (
              <Icon
                name={option.icon}
                size="medium"
                tone={isSelected ? "primary" : "muted-foreground"}
              />
            ) : null}
            <AppText variant="bodyStrong" tone={isSelected ? "primary" : "muted"} numberOfLines={1}>
              {option.label}
            </AppText>
          </Pressable>
        );
      })}
    </View>
  );
}
