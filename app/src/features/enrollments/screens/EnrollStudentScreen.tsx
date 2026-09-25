import { useRouter } from "expo-router";
import { useState } from "react";
import { ActivityIndicator, FlatList, Pressable, Text, View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { useClassGroupsIncludingInactive } from "@/features/classGroups/useClassGroups";
import { enrollmentErrorCodes } from "@/features/enrollments/enrollmentErrorCodes";
import { useClassRoster } from "@/features/enrollments/useClassRoster";
import { useEnrollStudent } from "@/features/enrollments/useEnrollmentMutations";
import { StudentSummary } from "@/features/students/types";
import { useStudentSearch } from "@/features/students/useStudentSearch";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { SearchInput } from "@/ui/SearchInput";
import { useToast } from "@/ui/ToastProvider";

interface EnrollStudentScreenProps {
  classGroupId: string;
}

type EnrollFailure = "classFull" | "alreadyEnrolled" | "unexpected";

const enrollFailureMessages = {
  classFull: "enrollments.enroll.classFull",
  alreadyEnrolled: "enrollments.enroll.alreadyEnrolledError",
  unexpected: "common.unexpectedError",
} as const;

export function EnrollStudentScreen({ classGroupId }: EnrollStudentScreenProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const [searchText, setSearchText] = useState("");
  const [enrollFailure, setEnrollFailure] = useState<EnrollFailure | null>(null);
  const { data: students = [], isPending } = useStudentSearch(searchText);
  const { data: roster = [] } = useClassRoster(classGroupId);
  const { data: classGroups = [] } = useClassGroupsIncludingInactive();
  const enrollStudentMutation = useEnrollStudent(classGroupId);
  const classGroupName =
    classGroups.find((classGroup) => classGroup.id === classGroupId)?.name ?? "";
  const enrolledStudentIds = new Set(roster.map((entry) => entry.studentId));

  const enroll = async (student: StudentSummary) => {
    setEnrollFailure(null);
    try {
      await enrollStudentMutation.mutateAsync({ studentId: student.id });
      showToast(
        translate("enrollments.enroll.success", {
          student: student.fullName,
          classGroup: classGroupName,
        }),
      );
      router.back();
    } catch (enrollError) {
      if (isApiError(enrollError) && enrollError.hasCode(enrollmentErrorCodes.classGroupFull)) {
        setEnrollFailure("classFull");
        return;
      }
      if (isApiError(enrollError) && enrollError.hasCode(enrollmentErrorCodes.alreadyEnrolled)) {
        setEnrollFailure("alreadyEnrolled");
        return;
      }
      setEnrollFailure("unexpected");
    }
  };

  return (
    <View className="flex-1 bg-gray-50">
      <View className="gap-3 p-4">
        <SearchInput
          value={searchText}
          onChangeText={setSearchText}
          placeholder={translate("students.list.searchPlaceholder")}
          autoFocus
        />
        {enrollFailure ? (
          <Banner tone="warning" message={translate(enrollFailureMessages[enrollFailure])} />
        ) : null}
      </View>
      {isPending ? (
        <ActivityIndicator className="mt-6" />
      ) : (
        <FlatList
          data={students}
          keyExtractor={(student) => student.id}
          renderItem={({ item: student }) => {
            const isEnrolled = enrolledStudentIds.has(student.id);
            return (
              <Pressable
                accessibilityRole="button"
                accessibilityLabel={student.fullName}
                accessibilityState={{ disabled: isEnrolled }}
                disabled={isEnrolled || enrollStudentMutation.isPending}
                onPress={() => enroll(student)}
                className={`flex-row items-center gap-3 border-b border-gray-100 bg-white px-4 py-3 ${isEnrolled ? "opacity-50" : ""}`}
              >
                <View className="flex-1 gap-1">
                  <Text className="text-base font-semibold text-gray-900">{student.fullName}</Text>
                  <Text className="text-sm text-gray-600">
                    {translate("students.list.responsible", { name: student.clientFullName })}
                  </Text>
                </View>
                {isEnrolled ? (
                  <Text className="text-sm text-gray-500">
                    {translate("enrollments.enroll.alreadyEnrolled")}
                  </Text>
                ) : null}
              </Pressable>
            );
          }}
          ListEmptyComponent={
            <Text className="p-6 text-center text-base text-gray-600">
              {translate("enrollments.enroll.noStudents")}
            </Text>
          }
        />
      )}
    </View>
  );
}
