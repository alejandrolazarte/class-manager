import { View } from "react-native";
import { MakeupSlotCard } from "@/features/family/components/MakeupSlotCard";
import { useFamilyPackClasses } from "@/features/family/useFamilyPackClasses";
import { usePackClassBooking } from "@/features/family/usePackClassBooking";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
import { SectionTitle } from "@/ui/SectionTitle";

interface PackClassSectionProps {
  studentId: string;
  days: string[];
}

export function PackClassSection({ studentId, days }: PackClassSectionProps) {
  const { data: packClasses } = useFamilyPackClasses(studentId);
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
    <View className="gap-3">
      <SectionTitle title={translate("family.packClasses.title")} />
      <Card className="flex-row items-center gap-3 px-4 py-3.5">
        <View className="h-10 w-10 items-center justify-center rounded-full bg-primary-soft">
          <Icon name="classPacks" tone="primary" />
        </View>
        <View className="min-w-0 flex-1 gap-0.5">
          <AppText variant="bodyStrong">
            {classesLeft > 0
              ? translateCount("family.packClasses.available", classesLeft)
              : translate("family.packClasses.none")}
          </AppText>
          <AppText variant="caption" tone="muted">
            {translate("family.packClasses.howTo")}
          </AppText>
        </View>
      </Card>
      {classesLeft > 0 && weekSlots.length === 0 ? (
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
            canBook={classesLeft > 0}
            isPending={pendingKey === key}
            cancelLabelKey="family.packClasses.cancel"
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
  );
}
