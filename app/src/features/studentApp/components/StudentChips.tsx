import { ScrollView } from "react-native";
import { firstNameOf } from "@/features/studentApp/studentAppSchedule";
import { AccountStudent } from "@/features/studentApp/types";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { PressableScale } from "@/ui/motion/PressableScale";
import { SlidingIndicator, useOptionLayouts } from "@/ui/motion/SlidingIndicator";

interface StudentChipsProps {
  students: readonly (Pick<AccountStudent, "id" | "fullName"> & { photoUrl?: string | null })[];
  selectedStudentId: string;
  onSelect: (studentId: string) => void;
}

export function StudentChips({ students, selectedStudentId, onSelect }: StudentChipsProps) {
  const { layouts, onOptionLayout } = useOptionLayouts<string>();
  if (students.length < 2) {
    return null;
  }
  return (
    <ScrollView
      horizontal
      showsHorizontalScrollIndicator={false}
      className="-mx-5"
      contentContainerClassName="relative gap-1.5 px-5"
    >
      <SlidingIndicator layout={layouts[selectedStudentId]} className="rounded-full bg-primary" />
      {students.map((student) => {
        const isSelected = student.id === selectedStudentId;
        const firstName = firstNameOf(student.fullName);
        return (
          <PressableScale
            key={student.id}
            accessibilityRole="button"
            accessibilityLabel={firstName}
            accessibilityState={{ selected: isSelected }}
            onPress={() => onSelect(student.id)}
            onLayout={onOptionLayout(student.id)}
            className={`h-10 flex-row items-center gap-2 rounded-full pl-[5px] pr-3.5 ${isSelected ? "" : "active:bg-muted"}`}
          >
            <Avatar
              name={student.fullName}
              size="tiny"
              tone={isSelected ? "onPrimary" : "primarySoft"}
              photoUrl={student.photoUrl}
            />
            <AppText variant="bodyStrong" tone={isSelected ? "onPrimary" : "default"}>
              {firstName}
            </AppText>
          </PressableScale>
        );
      })}
    </ScrollView>
  );
}
