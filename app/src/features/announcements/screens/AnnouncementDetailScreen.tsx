import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { announcementCategoryPresentations } from "@/features/announcements/announcementCategories";
import { Announcement, AnnouncementRecipient } from "@/features/announcements/types";
import {
  useAnnouncementRecipients,
  useAnnouncements,
  useRemindAnnouncement,
} from "@/features/announcements/useAnnouncements";
import { formatLongDate, toIsoDate } from "@/features/sessions/dates";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { translate, translateCount, TranslationKey } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { CountsRow, CountsRowItem } from "@/ui/CountsRow";
import { EmptyState } from "@/ui/EmptyState";
import { HairlineRow } from "@/ui/HairlineRow";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SegmentedControl, SegmentedOption } from "@/ui/SegmentedControl";
import { StatusPill } from "@/ui/StatusPill";
import { useToast } from "@/ui/ToastProvider";
import { AnimatedListItem } from "@/ui/motion/AnimatedListItem";
import { SkeletonList } from "@/ui/motion/SkeletonList";

type RecipientFilter = "unseen" | "seen" | "replies";

interface AnnouncementDetailScreenProps {
  announcementId: string;
}

interface AnnouncementRecipientsProps {
  announcement: Announcement;
}

const emptyMessageKeys: Record<RecipientFilter, TranslationKey> = {
  unseen: "announcements.detail.emptyUnseen",
  seen: "announcements.detail.emptySeen",
  replies: "announcements.detail.emptyReplies",
};

function matchesFilter(recipient: AnnouncementRecipient, filter: RecipientFilter): boolean {
  if (filter === "unseen") {
    return recipient.seenAt === null;
  }
  if (filter === "seen") {
    return recipient.seenAt !== null;
  }
  return recipient.isGoing !== null;
}

function RecipientRow({ recipient }: { recipient: AnnouncementRecipient }) {
  return (
    <HairlineRow>
      <Avatar name={recipient.fullName} size="small" />
      <View className="min-w-0 flex-1">
        <AppText variant="body">{recipient.fullName}</AppText>
        {recipient.seenAt === null ? null : (
          <AppText variant="footnote" tone="subtle">
            {translate("announcements.detail.seenOn", {
              date: formatLongDate(toIsoDate(new Date(recipient.seenAt))),
            })}
          </AppText>
        )}
      </View>
      {recipient.isGoing === null ? null : (
        <StatusPill
          label={translate(
            recipient.isGoing
              ? "announcements.detail.goingPill"
              : "announcements.detail.notGoingPill",
          )}
          tone={recipient.isGoing ? "success" : "neutral"}
          isSmall
        />
      )}
    </HairlineRow>
  );
}

function AnnouncementRecipients({ announcement }: AnnouncementRecipientsProps) {
  const { showToast } = useToast();
  const isEvent = announcement.category === "Event";
  const [filter, setFilter] = useState<RecipientFilter>("unseen");
  const {
    data: recipients = [],
    isPending,
    isError,
    refetch,
  } = useAnnouncementRecipients(announcement.id);
  const remindMutation = useRemindAnnouncement();
  const unseenCount = recipients.filter((recipient) => recipient.seenAt === null).length;
  const seenCount = recipients.length - unseenCount;
  const goingCount = recipients.filter((recipient) => recipient.isGoing === true).length;
  const notGoingCount = recipients.filter((recipient) => recipient.isGoing === false).length;

  const counts: CountsRowItem[] = [
    {
      key: "seen",
      count: seenCount,
      label: translate("announcements.detail.seen"),
      tone: "default",
    },
    {
      key: "unseen",
      count: unseenCount,
      label: translate("announcements.detail.unseen"),
      tone: "danger",
    },
    ...(isEvent
      ? [
          {
            key: "going",
            count: goingCount,
            label: translate("announcements.detail.going"),
            tone: "success" as const,
          },
          {
            key: "notGoing",
            count: notGoingCount,
            label: translate("announcements.detail.notGoing"),
            tone: "default" as const,
          },
        ]
      : []),
  ];
  const filterOptions: SegmentedOption<RecipientFilter>[] = [
    { value: "unseen", label: translate("announcements.detail.unseen") },
    { value: "seen", label: translate("announcements.detail.seen") },
    ...(isEvent
      ? [{ value: "replies" as const, label: translate("announcements.detail.replies") }]
      : []),
  ];
  const shownRecipients = recipients.filter((recipient) => matchesFilter(recipient, filter));

  const remind = async () => {
    try {
      const reminder = await remindMutation.mutateAsync(announcement.id);
      showToast(translateCount("announcements.reminded", reminder.remindedCount));
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  if (isPending) {
    return <SkeletonList />;
  }
  if (isError) {
    return (
      <Banner message={translate("common.unexpectedError")}>
        <Button
          variant="secondary"
          size="medium"
          label={translate("common.retry")}
          onPress={() => refetch()}
        />
      </Banner>
    );
  }
  if (recipients.length === 0) {
    return (
      <EmptyState
        icon="studentApp"
        iconTone="disabled-foreground"
        message={translate("announcements.detail.noRecipients")}
      />
    );
  }
  return (
    <View className="gap-4">
      <CountsRow items={counts} accessibilityLabel={translate("announcements.detail.counts")} />
      <Button
        variant="secondary"
        size="medium"
        icon="notifications"
        label={translate("announcements.remind")}
        disabled={unseenCount === 0}
        isLoading={remindMutation.isPending}
        onPress={remind}
      />
      <SegmentedControl options={filterOptions} selectedValue={filter} onChange={setFilter} />
      {shownRecipients.length === 0 ? (
        <EmptyState
          icon="students"
          iconTone="disabled-foreground"
          message={translate(emptyMessageKeys[filter])}
        />
      ) : (
        <View>
          {shownRecipients.map((recipient, index) => (
            <AnimatedListItem key={recipient.clientId} index={index}>
              <RecipientRow recipient={recipient} />
            </AnimatedListItem>
          ))}
        </View>
      )}
    </View>
  );
}

export function AnnouncementDetailScreen({ announcementId }: AnnouncementDetailScreenProps) {
  const router = useRouter();
  const announcementsQuery = useAnnouncements();
  const announcement = announcementsQuery.data?.find(
    (candidate) => candidate.id === announcementId,
  );
  if (announcement === undefined) {
    return (
      <SettingsItemState
        isPending={announcementsQuery.isPending}
        isError={announcementsQuery.isError}
        notFoundMessage={translate("announcements.detail.notFound")}
        onRetry={() => announcementsQuery.refetch()}
      />
    );
  }
  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="back"
          navigationAction={
            <Button
              variant="ghost"
              size="medium"
              icon="edit"
              label={translate("announcements.detail.edit")}
              onPress={() => router.push(routes.editAnnouncement(announcement.id))}
            />
          }
          eyebrow={translate(announcementCategoryPresentations[announcement.category].labelKey)}
          title={announcement.title}
        />
      }
    >
      <AnnouncementRecipients announcement={announcement} />
    </ScrollScreen>
  );
}
