import { Pressable, View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";
import { SlidingIndicator, useOptionLayouts } from "@/ui/motion/SlidingIndicator";

export interface TabOption<TValue extends string> {
  value: TValue;
  label: string;
  icon?: IconName;
  accessibilityLabel?: string;
}

const selectedTabLineHeight = 3;

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
  const { layouts, onOptionLayout } = useOptionLayouts<TValue>();
  const selectedLayout = layouts[selectedValue];
  return (
    <View accessibilityRole="tablist" className="relative flex-row border-b border-border">
      {options.map((option) => {
        const isSelected = option.value === selectedValue;
        return (
          <Pressable
            key={option.value}
            accessibilityRole="tab"
            accessibilityLabel={option.accessibilityLabel ?? option.label}
            accessibilityState={{ selected: isSelected }}
            onPress={() => onChange(option.value)}
            onLayout={onOptionLayout(option.value)}
            className={`h-12 flex-1 flex-row items-center justify-center gap-1.5 px-2 ${isSelected ? "" : "active:bg-muted"}`}
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
      <SlidingIndicator
        layout={
          selectedLayout && {
            ...selectedLayout,
            y: selectedLayout.y + selectedLayout.height - selectedTabLineHeight,
            height: selectedTabLineHeight,
          }
        }
        className="rounded-full bg-primary"
      />
    </View>
  );
}
