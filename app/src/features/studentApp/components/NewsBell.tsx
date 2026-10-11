import { View } from "react-native";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";
import { PressableScale } from "@/ui/motion/PressableScale";

interface NewsBellProps {
  unreadCount: number;
  onPress: () => void;
}

export function NewsBell({ unreadCount, onPress }: NewsBellProps) {
  return (
    <PressableScale
      accessibilityRole="button"
      accessibilityLabel={
        unreadCount > 0
          ? translateCount("student.news.bell", unreadCount)
          : translate("student.news.bellEmpty")
      }
      onPress={onPress}
      className="-mr-2.5 h-11 w-11 items-center justify-center rounded-full active:bg-muted"
    >
      <Icon name="notifications" />
      {unreadCount > 0 ? (
        <View className="absolute right-1.5 top-1.5 h-[18px] min-w-[18px] items-center justify-center rounded-full border-2 border-background bg-danger px-0.5">
          <AppText variant="counter" tone="onDanger">
            {unreadCount}
          </AppText>
        </View>
      ) : null}
    </PressableScale>
  );
}
