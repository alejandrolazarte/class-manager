import { View } from "react-native";
import { useReplyToAnnouncement } from "@/features/studentApp/useStudentAppNews";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { useToast } from "@/ui/ToastProvider";

interface EventReplyButtonsProps {
  announcementId: string;
  isGoing: boolean | null;
}

export function EventReplyButtons({ announcementId, isGoing }: EventReplyButtonsProps) {
  const { showToast } = useToast();
  const replyMutation = useReplyToAnnouncement();

  const reply = async (answer: boolean) => {
    try {
      await replyMutation.mutateAsync({ announcementId, isGoing: answer });
      showToast(translate("student.news.reply.saved"));
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  return (
    <View className="mt-2 gap-1.5">
      <AppText variant="label" tone="muted">
        {translate("student.news.reply.question")}
      </AppText>
      <View className="flex-row gap-2">
        <Button
          size="small"
          variant={isGoing === true ? "primary" : "secondary"}
          icon={isGoing === true ? "checked" : undefined}
          label={translate("student.news.reply.going")}
          disabled={replyMutation.isPending}
          onPress={() => reply(true)}
        />
        <Button
          size="small"
          variant={isGoing === false ? "primary" : "secondary"}
          icon={isGoing === false ? "checked" : undefined}
          label={translate("student.news.reply.notGoing")}
          disabled={replyMutation.isPending}
          onPress={() => reply(false)}
        />
      </View>
    </View>
  );
}
