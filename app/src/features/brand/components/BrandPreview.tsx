import { View } from "react-native";
import { translate } from "@/i18n/translate";
import { TabIcon } from "@/navigation/tabBar";
import { ThemeColors } from "@/theme/themeColorTokens";
import { ThemePreviewScope } from "@/theme/ThemePreviewScope";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { StatusPill } from "@/ui/StatusPill";

const previewTabs = [
  { icon: "today", labelKey: "tabs.today" },
  { icon: "students", labelKey: "tabs.students" },
  { icon: "fees", labelKey: "tabs.fees" },
  { icon: "settings", labelKey: "tabs.settings" },
] as const;

export function BrandPreview({ colors }: { colors: ThemeColors }) {
  return (
    <ThemePreviewScope colors={colors} className="gap-3 rounded-[20px] bg-background p-4">
      <AppText variant="eyebrow" tone="accent">
        {translate("brand.settings.previewEyebrow")}
      </AppText>
      <Button label={translate("brand.settings.previewAction")} onPress={() => undefined} />
      <View className="flex-row gap-2">
        <View className="rounded-full bg-accent-soft px-3 py-1">
          <AppText variant="badge" tone="accentSoft">
            {translate("brand.settings.previewToday")}
          </AppText>
        </View>
        <StatusPill label={translate("brand.settings.previewPaid")} tone="success" />
      </View>
      <View className="flex-row justify-between rounded-2xl bg-surface px-2 py-2.5">
        {previewTabs.map((tab, index) => (
          <View key={tab.icon} className="items-center gap-1">
            <TabIcon icon={tab.icon} isFocused={index === 0} />
            <AppText variant="badge" tone={index === 0 ? "default" : "muted"}>
              {translate(tab.labelKey)}
            </AppText>
          </View>
        ))}
      </View>
    </ThemePreviewScope>
  );
}
