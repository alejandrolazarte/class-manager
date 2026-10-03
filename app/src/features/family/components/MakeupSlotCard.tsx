import { View } from "react-native";
import { shortDayLabel } from "@/features/family/familySchedule";
import { FamilyMakeupSlot } from "@/features/family/types";
import { translate, translateCount, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";

const detailSeparator = " · ";

interface MakeupSlotCardProps {
  slot: FamilyMakeupSlot;
  canBook: boolean;
  isPending: boolean;
  cancelLabelKey?: TranslationKey;
  onToggle: () => void;
}

function spotsLabel(spotsLeft: number): string {
  return spotsLeft === 0
    ? translate("family.makeup.spots.none")
    : translateCount("family.makeup.spots", spotsLeft);
}

export function MakeupSlotCard({
  slot,
  canBook,
  isPending,
  cancelLabelKey = "family.makeup.cancel",
  onToggle,
}: MakeupSlotCardProps) {
  const isFull = slot.spotsLeft === 0 && !slot.isBooked;
  return (
    <Card className="flex-row items-center gap-3 px-4 py-3.5">
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="headline">
          {`${shortDayLabel(slot.date)} · ${slot.startTime}–${slot.endTime}`}
        </AppText>
        <AppText variant="body" tone="muted">
          {[slot.name, slot.instructorFullName, spotsLabel(slot.spotsLeft)]
            .filter(Boolean)
            .join(detailSeparator)}
        </AppText>
      </View>
      <Button
        size="medium"
        variant={slot.isBooked ? "secondary" : "primary"}
        icon={slot.isBooked ? "present" : "add"}
        label={translate(slot.isBooked ? "family.makeup.booked" : "family.makeup.book")}
        accessibilityLabel={
          slot.isBooked
            ? `${translate(cancelLabelKey)} ${shortDayLabel(slot.date)} ${slot.startTime}`
            : undefined
        }
        disabled={isFull || (!slot.isBooked && !canBook)}
        isLoading={isPending}
        onPress={onToggle}
      />
    </Card>
  );
}
