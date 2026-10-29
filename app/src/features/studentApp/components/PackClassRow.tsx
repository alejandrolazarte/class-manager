import { View } from "react-native";
import { dayAndMonthLabel } from "@/features/studentApp/studentAppSchedule";
import { StudentAppPackClass } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";

interface PackClassRowProps {
  packClass: StudentAppPackClass;
  studentName: string | null;
  hasDivider: boolean;
}

const detailSeparator = " · ";

function classNameOf(packClass: StudentAppPackClass): string {
  return packClass.isPrivateLesson
    ? translate("privateLessons.withInstructor", { instructor: packClass.className })
    : packClass.className;
}

export function PackClassRow({ packClass, studentName, hasDivider }: PackClassRowProps) {
  return (
    <HairlineRow hasDivider={hasDivider}>
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong">{dayAndMonthLabel(packClass.date)}</AppText>
        <AppText variant="caption" tone="muted">
          {[classNameOf(packClass), studentName]
            .filter((part) => part !== null)
            .join(detailSeparator)}
        </AppText>
      </View>
    </HairlineRow>
  );
}
