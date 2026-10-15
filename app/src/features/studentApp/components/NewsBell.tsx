import { View } from "react-native";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { IconButton } from "@/ui/IconButton";

interface NewsBellProps {
  unreadCount: number;
  onPress: () => void;
}

export function NewsBell({ unreadCount, onPress }: NewsBellProps) {
  return (
    <View className="-mr-2.5">
      <IconButton
        icon="notifications"
        accessibilityLabel={
          unreadCount > 0
            ? translateCount("student.news.bell", unreadCount)
            : translate("student.news.bellEmpty")
        }
        onPress={onPress}
      />
      {unreadCount > 0 ? (
        <View
          pointerEvents="none"
          className="absolute right-1.5 top-1.5 h-[18px] min-w-[18px] items-center justify-center rounded-full border-2 border-background bg-danger px-0.5"
        >
          <AppText variant="counter" tone="onDanger">
            {unreadCount}
          </AppText>
        </View>
      ) : null}
    </View>
  );
}
