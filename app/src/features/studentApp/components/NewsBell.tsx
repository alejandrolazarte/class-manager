import { Pressable, View } from "react-native";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { useElevationStyle } from "@/ui/elevation";
import { Icon } from "@/ui/Icon";

interface NewsBellProps {
  unreadCount: number;
  onPress: () => void;
}

export function NewsBell({ unreadCount, onPress }: NewsBellProps) {
  const elevationStyle = useElevationStyle();
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={
        unreadCount > 0
          ? translateCount("student.news.bell", unreadCount)
          : translate("student.news.bellEmpty")
      }
      onPress={onPress}
      style={elevationStyle}
      className="h-11 w-11 items-center justify-center rounded-full bg-surface active:bg-muted"
    >
      <Icon name="notifications" />
      {unreadCount > 0 ? (
        <View className="absolute right-1.5 top-1.5 h-[18px] min-w-[18px] items-center justify-center rounded-full border-2 border-surface bg-danger px-0.5">
          <AppText variant="counter" tone="onDanger">
            {unreadCount}
          </AppText>
        </View>
      ) : null}
    </Pressable>
  );
}
