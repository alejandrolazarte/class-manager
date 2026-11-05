import { View } from "react-native";
import {
  NotificationAudience,
  NotificationPreference,
} from "@/features/notifications/notificationPreferenceTypes";
import {
  useNotificationPreferences,
  useUpdateNotificationPreference,
} from "@/features/notifications/useNotificationPreferences";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { HairlineRow } from "@/ui/HairlineRow";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { ToggleSwitch } from "@/ui/ToggleSwitch";
import { SkeletonDetail } from "@/ui/motion/SkeletonDetail";
import { useToast } from "@/ui/ToastProvider";

const audiences: NotificationAudience[] = ["Team", "Family"];

function categoryName(preference: NotificationPreference): string {
  return translate(`notificationPreferences.category.${preference.category}` as TranslationKey);
}

interface CategoryRowProps {
  preference: NotificationPreference;
  hasDivider: boolean;
  onChange: (
    channel: NotificationPreference["channels"][number]["channel"],
    isEnabled: boolean,
  ) => void;
}

function CategoryRow({ preference, hasDivider, onChange }: CategoryRowProps) {
  const name = categoryName(preference);
  return (
    <HairlineRow hasDivider={hasDivider} className="flex-col items-stretch gap-1 py-3">
      <AppText variant="bodyStrong">{name}</AppText>
      <AppText variant="caption" tone="subtle">
        {translate(`notificationPreferences.description.${preference.category}` as TranslationKey)}
      </AppText>
      {preference.channels.map(({ channel, isEnabled }) => {
        const channelName = translate(
          `notificationPreferences.channel.${channel}` as TranslationKey,
        );
        return (
          <ToggleSwitch
            key={channel}
            label={channelName}
            accessibilityLabel={translate("notificationPreferences.channelAccessibility", {
              category: name,
              channel: channelName,
            })}
            value={isEnabled}
            onValueChange={(turnOn) => onChange(channel, turnOn)}
          />
        );
      })}
    </HairlineRow>
  );
}

export function NotificationPreferencesScreen() {
  const { showToast } = useToast();
  const preferencesQuery = useNotificationPreferences();
  const updatePreference = useUpdateNotificationPreference(() =>
    showToast(translate("common.unexpectedError")),
  );
  const preferences = preferencesQuery.data;

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="back" title={translate("notificationPreferences.title")} />}
    >
      <AppText variant="body" tone="muted">
        {translate("notificationPreferences.hint")}
      </AppText>
      {preferencesQuery.isPending ? <SkeletonDetail /> : null}
      {preferencesQuery.isError ? (
        <Banner message={translate("common.unexpectedError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => preferencesQuery.refetch()}
          />
        </Banner>
      ) : null}
      {preferences
        ? audiences.map((audience) => {
            const group = preferences.filter((preference) => preference.audience === audience);
            if (group.length === 0) {
              return null;
            }
            return (
              <View key={audience} className="gap-0.5">
                <SectionTitle
                  title={translate(
                    `notificationPreferences.audience.${audience}` as TranslationKey,
                  )}
                  isOverline
                />
                {audience === "Team" ? (
                  <AppText variant="caption" tone="subtle">
                    {translate("notificationPreferences.teamHint")}
                  </AppText>
                ) : null}
                {group.map((preference, index) => (
                  <CategoryRow
                    key={preference.category}
                    preference={preference}
                    hasDivider={index < group.length - 1}
                    onChange={(channel, isEnabled) =>
                      updatePreference.mutate({ category: preference.category, channel, isEnabled })
                    }
                  />
                ))}
              </View>
            );
          })
        : null}
    </ScrollScreen>
  );
}
