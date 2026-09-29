import { Pressable, ScrollView } from "react-native";
import { firstNameOf } from "@/features/family/familySchedule";
import { FamilyStudent } from "@/features/family/types";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";

interface StudentChipsProps {
  students: readonly FamilyStudent[];
  selectedStudentId: string;
  onSelect: (studentId: string) => void;
}

export function StudentChips({ students, selectedStudentId, onSelect }: StudentChipsProps) {
  if (students.length < 2) {
    return null;
  }
  return (
    <ScrollView
      horizontal
      showsHorizontalScrollIndicator={false}
      className="-mx-5"
      contentContainerClassName="gap-2 px-5"
    >
      {students.map((student) => {
        const isSelected = student.id === selectedStudentId;
        const firstName = firstNameOf(student.fullName);
        return (
          <Pressable
            key={student.id}
            accessibilityRole="button"
            accessibilityLabel={firstName}
            accessibilityState={{ selected: isSelected }}
            onPress={() => onSelect(student.id)}
            className={`h-11 flex-row items-center gap-2 rounded-full border-[1.5px] pl-[5px] pr-3.5 ${isSelected ? "border-primary bg-primary" : "border-border bg-surface active:bg-muted"}`}
          >
            <Avatar
              name={student.fullName}
              size="tiny"
              tone={isSelected ? "onPrimary" : "primarySoft"}
            />
            <AppText variant="link" tone={isSelected ? "onPrimary" : "default"}>
              {firstName}
            </AppText>
          </Pressable>
        );
      })}
    </ScrollView>
  );
}
