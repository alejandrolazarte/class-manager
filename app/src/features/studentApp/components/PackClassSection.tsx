import { View } from "react-native";
import { MakeupSlotRow } from "@/features/studentApp/components/MakeupSlotRow";
import { useStudentAppPackClasses } from "@/features/studentApp/useStudentAppPackClasses";
import { usePackClassBooking } from "@/features/studentApp/usePackClassBooking";
import { AccentNote } from "@/ui/AccentNote";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { SectionTitle } from "@/ui/SectionTitle";

interface PackClassSectionProps {
  studentId: string;
  days: string[];
}

export function PackClassSection({ studentId, days }: PackClassSectionProps) {
  const { data: packClasses } = useStudentAppPackClasses(studentId);
  const { togglePackClass, pendingKey } = usePackClassBooking();
  if (
    packClasses === undefined ||
    (packClasses.slots.length === 0 && packClasses.classesLeft === 0)
  ) {
    return null;
  }

  const { classesLeft } = packClasses;
  const weekSlots = packClasses.slots.filter((slot) => days.includes(slot.date));
  const visibleSlots = classesLeft > 0 ? weekSlots : weekSlots.filter((slot) => slot.isBooked);
  return (
    <View className="gap-2">
      <SectionTitle title={translate("student.packClasses.title")} />
      <AccentNote tone="primary">
        <AppText variant="bodyStrong">
          {classesLeft > 0
            ? translateCount("student.packClasses.available", classesLeft)
            : translate("student.packClasses.none")}
        </AppText>
        <AppText variant="caption" tone="muted">
          {translate("student.packClasses.howTo")}
        </AppText>
      </AccentNote>
      {classesLeft > 0 && weekSlots.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate("student.makeup.emptyWeek")}
        </AppText>
      ) : null}
      <View>
        {visibleSlots.map((slot) => {
          const key = `${slot.classGroupId}-${slot.date}`;
          return (
            <MakeupSlotRow
              key={key}
              slot={slot}
              canBook={classesLeft > 0}
              isPending={pendingKey === key}
              cancelLabelKey="student.packClasses.cancel"
              onToggle={() =>
                togglePackClass({
                  studentId,
                  classGroupId: slot.classGroupId,
                  date: slot.date,
                  isBooked: slot.isBooked,
                })
              }
            />
          );
        })}
      </View>
    </View>
  );
}
