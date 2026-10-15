import { View } from "react-native";
import { AppText } from "@/ui/AppText";
import { IconCircle } from "@/ui/IconCircle";

interface PushNotificationPreviewProps {
  appName: string;
  title: string;
  body: string | null;
}

export function PushNotificationPreview({ appName, title, body }: PushNotificationPreviewProps) {
  return (
    <View
      testID="push-notification-preview"
      className="flex-row gap-3 rounded-2xl border border-border bg-surface px-3.5 py-3"
    >
      <IconCircle icon="notifications" tone="primary" />
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="footnote" tone="subtle">
          {appName}
        </AppText>
        <AppText variant="bodyStrong" numberOfLines={1}>
          {title}
        </AppText>
        {body === null ? null : (
          <AppText variant="caption" tone="muted" numberOfLines={3}>
            {body}
          </AppText>
        )}
      </View>
    </View>
  );
}
