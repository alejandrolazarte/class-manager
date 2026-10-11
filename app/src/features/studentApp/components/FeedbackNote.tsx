import { firstNameOf, shortDayLabel } from "@/features/studentApp/studentAppSchedule";
import { StudentAppFeedback } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";
import { AccentNote } from "@/ui/AccentNote";
import { AppText } from "@/ui/AppText";

export function FeedbackNote({ feedback }: { feedback: StudentAppFeedback }) {
  return (
    <AccentNote tone="primary">
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
    </AccentNote>
  );
}
