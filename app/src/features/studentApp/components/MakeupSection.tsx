import { View } from "react-native";
import { MakeupSlotCard } from "@/features/studentApp/components/MakeupSlotCard";
import { dayAndMonthLabel } from "@/features/studentApp/studentAppSchedule";
import { useStudentAppMakeups } from "@/features/studentApp/useStudentAppMakeups";
import { useMakeupBooking } from "@/features/studentApp/useMakeupBooking";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
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
    <View className="gap-3">
      <SectionTitle title={translate("student.makeup.title")} />
      <Card className="flex-row items-center gap-3 px-4 py-3.5">
        <View className="h-10 w-10 items-center justify-center rounded-full bg-warning-soft">
          <Icon name="refund" tone="warning" />
        </View>
        <View className="min-w-0 flex-1 gap-0.5">
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
        </View>
      </Card>
      {credits > 0 && weekSlots.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate("student.makeup.emptyWeek")}
        </AppText>
      ) : null}
      {visibleSlots.map((slot) => {
        const key = `${slot.classGroupId}-${slot.date}`;
        return (
          <MakeupSlotCard
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
  );
}
