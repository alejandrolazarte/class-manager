import { formatMinutesOfDay } from "@/features/classGroups/startTimeFormatting";
import { FreeGap } from "@/features/privateLessons/instructorDay";
import { translate } from "@/i18n/translate";

export function freeGapLabel(gap: FreeGap): string {
  if (gap.isOpenStart) {
    return translate("privateLessons.agenda.gapUntil", {
      time: formatMinutesOfDay(gap.endMinutes),
    });
  }
  if (gap.isOpenEnd) {
    return translate("privateLessons.agenda.gapFrom", {
      time: formatMinutesOfDay(gap.startMinutes),
    });
  }
  return translate("privateLessons.agenda.gapBetween", {
    start: formatMinutesOfDay(gap.startMinutes),
    end: formatMinutesOfDay(gap.endMinutes),
  });
}
