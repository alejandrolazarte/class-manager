import { View } from "react-native";
import { useReplyToAnnouncement } from "@/features/studentApp/useStudentAppNews";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { AnswerButton, AnswerState } from "@/ui/AnswerButton";
import { useToast } from "@/ui/ToastProvider";

interface EventReplyButtonsProps {
  announcementId: string | null;
  isGoing: boolean | null;
}

function answerStateOf(isGoing: boolean | null, answer: boolean): AnswerState {
  if (isGoing === null) {
    return "open";
  }
  return isGoing === answer ? "chosen" : "notChosen";
}

export function EventReplyButtons({ announcementId, isGoing }: EventReplyButtonsProps) {
  const { showToast } = useToast();
  const replyMutation = useReplyToAnnouncement();

  const reply = async (answer: boolean) => {
    if (announcementId === null) {
      return;
    }
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
        <AnswerButton
          answer="yes"
          state={answerStateOf(isGoing, true)}
          label={translate("student.news.reply.going")}
          disabled={replyMutation.isPending}
          onPress={() => reply(true)}
        />
        <AnswerButton
          answer="no"
          state={answerStateOf(isGoing, false)}
          label={translate("student.news.reply.notGoing")}
          disabled={replyMutation.isPending}
          onPress={() => reply(false)}
        />
      </View>
    </View>
  );
}
