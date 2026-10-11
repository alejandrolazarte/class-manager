import { View } from "react-native";
import { MakeupSlotRow } from "@/features/studentApp/components/MakeupSlotRow";
import { dayAndMonthLabel } from "@/features/studentApp/studentAppSchedule";
import { useStudentAppMakeups } from "@/features/studentApp/useStudentAppMakeups";
import { useMakeupBooking } from "@/features/studentApp/useMakeupBooking";
import { AccentNote } from "@/ui/AccentNote";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { SectionTitle } from "@/ui/SectionTitle";

interface MakeupSectionProps {
  studentId: string;
  days: string[];
}

export function MakeupSection({ studentId, days }: MakeupSectionProps) {
  const { data: makeups } = useStudentAppMakeups(studentId);
  const { toggleMakeup, pendingKey } = useMakeupBooking();
  if (makeups === undefined) {
    return null;
  }

  const credits = makeups.credits.length;
  const weekSlots = makeups.slots.filter((slot) => days.includes(slot.date));
  const visibleSlots = credits > 0 ? weekSlots : weekSlots.filter((slot) => slot.isBooked);
  const firstExpiry = makeups.credits[0]?.expiresOn;
  return (
    <View className="gap-2">
      <SectionTitle title={translate("student.makeup.title")} />
      <AccentNote tone="warning">
        <AppText variant="bodyStrong">
          {credits > 0
            ? translateCount("student.makeup.available", credits)
            : translate("student.makeup.none")}
        </AppText>
        <AppText variant="caption" tone="muted">
          {firstExpiry === undefined
            ? translate("student.makeup.howTo")
            : translate("student.makeup.expires", { date: dayAndMonthLabel(firstExpiry) })}
        </AppText>
      </AccentNote>
      {credits > 0 && weekSlots.length === 0 ? (
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
              canBook={credits > 0}
              isPending={pendingKey === key}
              onToggle={() =>
                toggleMakeup({
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
