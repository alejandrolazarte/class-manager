import { ScrollView } from "react-native";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";
import { PressableScale } from "@/ui/motion/PressableScale";

export interface FilterBarItem {
  key: string;
  label: string;
  isActive: boolean;
  onPress: () => void;
}

interface FilterBarProps {
  items: readonly FilterBarItem[];
  onClear: () => void;
}

const buttonClassName = "h-10 flex-row items-center gap-1.5 rounded-full border-[1.5px] px-3.5";

export function FilterBar({ items, onClear }: FilterBarProps) {
  const hasActiveFilter = items.some((item) => item.isActive);
  return (
    <ScrollView
      horizontal
      showsHorizontalScrollIndicator={false}
      keyboardShouldPersistTaps="handled"
      className="-mx-5"
      contentContainerClassName="gap-2 px-5"
    >
      {hasActiveFilter ? (
        <PressableScale
          accessibilityRole="button"
          accessibilityLabel={translate("filters.clear")}
          onPress={onClear}
          className={`${buttonClassName} border-inverse bg-inverse`}
        >
          <Icon name="close" size="small" tone="inverse-foreground" />
          <AppText variant="link" tone="inverse" numberOfLines={1}>
            {translate("filters.clear")}
          </AppText>
        </PressableScale>
      ) : null}
      {items.map((item) => (
        <PressableScale
          key={item.key}
          accessibilityRole="button"
          accessibilityLabel={item.label}
          accessibilityState={{ selected: item.isActive }}
          onPress={item.onPress}
          className={`${buttonClassName} ${item.isActive ? "border-primary bg-primary-soft" : "border-border bg-surface active:bg-muted"}`}
        >
          <Icon
            name="filter"
            size="small"
            tone={item.isActive ? "primary-soft-foreground" : "foreground"}
          />
          <AppText
            variant="link"
            tone={item.isActive ? "primarySoft" : "default"}
            numberOfLines={1}
          >
            {item.label}
          </AppText>
        </PressableScale>
      ))}
    </ScrollView>
  );
}
