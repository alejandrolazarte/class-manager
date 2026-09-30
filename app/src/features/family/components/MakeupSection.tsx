import { View } from "react-native";
import { MakeupSlotCard } from "@/features/family/components/MakeupSlotCard";
import { dayAndMonthLabel } from "@/features/family/familySchedule";
import { useFamilyMakeups } from "@/features/family/useFamilyMakeups";
import { useMakeupBooking } from "@/features/family/useMakeupBooking";
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
  const { data: makeups } = useFamilyMakeups(studentId);
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
      <SectionTitle title={translate("family.makeup.title")} />
      <Card className="flex-row items-center gap-3 px-4 py-3.5">
        <View className="h-10 w-10 items-center justify-center rounded-full bg-warning-soft">
          <Icon name="refund" tone="warning" />
        </View>
        <View className="min-w-0 flex-1 gap-0.5">
          <AppText variant="bodyStrong">
            {credits > 0
              ? translateCount("family.makeup.available", credits)
              : translate("family.makeup.none")}
          </AppText>
          <AppText variant="caption" tone="muted">
            {firstExpiry === undefined
              ? translate("family.makeup.howTo")
              : translate("family.makeup.expires", { date: dayAndMonthLabel(firstExpiry) })}
          </AppText>
        </View>
      </Card>
      {credits > 0 && weekSlots.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate("family.makeup.emptyWeek")}
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
