import { View } from "react-native";
import { firstNameOf, shortDayLabel } from "@/features/studentApp/studentAppSchedule";
import { StudentAppFeedback } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Card } from "@/ui/Card";

export function FeedbackCard({ feedback }: { feedback: StudentAppFeedback }) {
  return (
    <Card className="gap-3 p-4">
      <View className="flex-row items-center gap-2.5">
        <Avatar
          name={feedback.instructorFullName ?? feedback.className}
          tone="success"
          size="small"
        />
        <View className="min-w-0 flex-1">
          <AppText variant="bodyStrong">
            {feedback.instructorFullName === null
              ? translate("student.feedback.titleWithoutInstructor")
              : translate("student.feedback.title", {
                  instructor: firstNameOf(feedback.instructorFullName),
                })}
          </AppText>
          <AppText variant="caption" tone="subtle">
            {translate("student.feedback.meta", {
              date: shortDayLabel(feedback.date),
              className: feedback.className,
            })}
          </AppText>
        </View>
      </View>
      <AppText variant="lead">{`“${feedback.text}”`}</AppText>
    </Card>
  );
}
