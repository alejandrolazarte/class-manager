import { firstNameOf, shortDayLabel } from "@/features/studentApp/studentAppSchedule";
import { StudentAppFeedback } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";
import { View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";

export function FeedbackNote({ feedback }: { feedback: StudentAppFeedback }) {
  return (
    <View className="flex-row gap-3">
      {feedback.instructorFullName === null ? null : (
        <Avatar
          name={feedback.instructorFullName}
          size="small"
          photoUrl={feedback.instructorPhotoUrl}
        />
      )}
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong">
          {feedback.instructorFullName === null
            ? translate("student.feedback.titleWithoutInstructor")
            : translate("student.feedback.title", {
                instructor: firstNameOf(feedback.instructorFullName),
              })}
        </AppText>
        <AppText variant="body" tone="muted">{`“${feedback.text}”`}</AppText>
        <AppText variant="footnote" tone="subtle" className="mt-1">
          {translate("student.feedback.meta", {
            date: shortDayLabel(feedback.date),
            className: feedback.className,
          })}
        </AppText>
      </View>
    </View>
  );
}
