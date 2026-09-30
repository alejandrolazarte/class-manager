import { View } from "react-native";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { Icon, IconName } from "@/ui/Icon";

interface HomeTileProps {
  icon: IconName;
  iconTone: ThemeColorToken;
  title: string;
  value: string;
  caption: string;
  onPress?: () => void;
}

export function HomeTile({ icon, iconTone, title, value, caption, onPress }: HomeTileProps) {
  return (
    <View className="flex-1">
      <Card
        onPress={onPress}
        accessibilityLabel={onPress === undefined ? undefined : `${title}: ${value}`}
        className="flex-1 gap-2 p-3.5"
      >
        <View className="flex-row items-center gap-1.5">
          <Icon name={icon} tone={iconTone} />
          <AppText variant="overline" tone="subtle">
            {title}
          </AppText>
        </View>
        <AppText variant="headline">{value}</AppText>
        <AppText variant="caption" tone="subtle">
          {caption}
        </AppText>
      </Card>
    </View>
  );
}
