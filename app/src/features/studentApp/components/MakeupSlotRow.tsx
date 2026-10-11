import { View } from "react-native";
import { shortDayLabel } from "@/features/studentApp/studentAppSchedule";
import { StudentAppMakeupSlot } from "@/features/studentApp/types";
import { translate, translateCount, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { HairlineRow } from "@/ui/HairlineRow";

const detailSeparator = " · ";

interface MakeupSlotRowProps {
  slot: StudentAppMakeupSlot;
  canBook: boolean;
  isPending: boolean;
  cancelLabelKey?: TranslationKey;
  onToggle: () => void;
}

function spotsLabel(spotsLeft: number): string {
  return spotsLeft === 0
    ? translate("student.makeup.spots.none")
    : translateCount("student.makeup.spots", spotsLeft);
}

export function MakeupSlotRow({
  slot,
  canBook,
  isPending,
  cancelLabelKey = "student.makeup.cancel",
  onToggle,
}: MakeupSlotRowProps) {
  const isFull = slot.spotsLeft === 0 && !slot.isBooked;
  return (
    <HairlineRow className="flex-row items-center gap-3">
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong">
          {`${shortDayLabel(slot.date)} · ${slot.startTime}–${slot.endTime}`}
        </AppText>
        <AppText variant="caption" tone="muted">
          {[slot.name, slot.instructorFullName, spotsLabel(slot.spotsLeft)]
            .filter(Boolean)
            .join(detailSeparator)}
        </AppText>
      </View>
      <Button
        size="medium"
        variant={slot.isBooked ? "secondary" : "primary"}
        icon={slot.isBooked ? "present" : "add"}
        label={translate(slot.isBooked ? "student.makeup.booked" : "student.makeup.book")}
        accessibilityLabel={
          slot.isBooked
            ? `${translate(cancelLabelKey)} ${shortDayLabel(slot.date)} ${slot.startTime}`
            : undefined
        }
        disabled={isFull || (!slot.isBooked && !canBook)}
        isLoading={isPending}
        onPress={onToggle}
      />
    </HairlineRow>
  );
}
