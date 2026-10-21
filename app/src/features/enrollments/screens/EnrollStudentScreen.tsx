import { useRouter } from "expo-router";
import { useState } from "react";
import { FlatList, View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { useClassGroupsIncludingInactive } from "@/features/classGroups/useClassGroups";
import { enrollmentErrorCodes } from "@/features/enrollments/enrollmentErrorCodes";
import { useClassRoster } from "@/features/enrollments/useClassRoster";
import { useEnrollStudent } from "@/features/enrollments/useEnrollmentMutations";
import { StudentSummary } from "@/features/students/types";
import { useStudentSearch } from "@/features/students/useStudentSearch";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
import { SearchInput } from "@/ui/SearchInput";
import { useToast } from "@/ui/ToastProvider";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { EmptyState } from "@/ui/EmptyState";
import { HairlineRow } from "@/ui/HairlineRow";
import { Screen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { studentAgeLabel } from "@/features/students/studentAgeLabel";
import { normalizeStudentName } from "@/features/students/studentSchema";
import { SkeletonList } from "@/ui/motion/SkeletonList";

interface EnrollStudentScreenProps {
  classGroupId: string;
}

const detailSeparator = " · ";

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
  const [studentOutsidePack, setStudentOutsidePack] = useState<StudentSummary | null>(null);
  const { data: students = [], isPending } = useStudentSearch(searchText);
  const { data: roster = [] } = useClassRoster(classGroupId);
  const { data: classGroups = [] } = useClassGroupsIncludingInactive();
  const enrollStudentMutation = useEnrollStudent(classGroupId);
  const classGroupName =
    classGroups.find((classGroup) => classGroup.id === classGroupId)?.name ?? "";
  const enrolledStudentIds = new Set(
    roster.filter((entry) => entry.endDate === null).map((entry) => entry.studentId),
  );

  const enroll = async (student: StudentSummary, allowOutsidePack = false) => {
    setEnrollFailure(null);
    try {
      await enrollStudentMutation.mutateAsync(
        allowOutsidePack ? { studentId: student.id, allowOutsidePack } : { studentId: student.id },
      );
      setStudentOutsidePack(null);
      showToast(
        translate("enrollments.enroll.success", {
          student: student.fullName,
          classGroup: classGroupName,
        }),
      );
      router.back();
    } catch (enrollError) {
      setStudentOutsidePack(null);
      if (isApiError(enrollError) && enrollError.hasCode(enrollmentErrorCodes.outsidePack)) {
        setStudentOutsidePack(student);
        return;
      }
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
    <Screen>
      <View className="gap-3.5 pb-3.5">
        <ScreenHeader
          navigation="close"
          eyebrow={classGroupName}
          title={translate("enrollments.enroll.title")}
        />
        <View className="gap-3.5 px-5">
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
      </View>
      {isPending ? (
        <SkeletonList className="px-5" />
      ) : (
        <FlatList
          data={students}
          keyExtractor={(student) => student.id}
          keyboardShouldPersistTaps="handled"
          contentContainerClassName="w-full max-w-2xl self-center px-5 pb-8"
          renderItem={({ item: student }) => {
            const isEnrolled = enrolledStudentIds.has(student.id);
            const isOwnClient =
              normalizeStudentName(student.fullName) ===
              normalizeStudentName(student.clientFullName);
            const details = [
              studentAgeLabel(student.birthDate),
              isOwnClient
                ? null
                : translate("students.list.responsible", { name: student.clientFullName }),
            ].filter((detail): detail is string => Boolean(detail));
            return (
              <HairlineRow
                accessibilityLabel={student.fullName}
                accessibilityState={{ disabled: isEnrolled }}
                disabled={isEnrolled || enrollStudentMutation.isPending}
                isDimmed={isEnrolled}
                onPress={() => enroll(student)}
              >
                <Avatar name={student.fullName} size="small" tone="muted" />
                <View className="min-w-0 flex-1 gap-0.5">
                  <AppText variant="bodyStrong">{student.fullName}</AppText>
                  {details.length > 0 ? (
                    <AppText variant="caption" tone="subtle">
                      {details.join(detailSeparator)}
                    </AppText>
                  ) : null}
                </View>
                <AppText variant="badge" tone={isEnrolled ? "subtle" : "primary"}>
                  {translate(
                    isEnrolled ? "enrollments.enroll.alreadyEnrolled" : "enrollments.enroll.enroll",
                  )}
                </AppText>
              </HairlineRow>
            );
          }}
          ListEmptyComponent={
            <EmptyState icon="students" message={translate("enrollments.enroll.noStudents")} />
          }
        />
      )}
      {studentOutsidePack ? (
        <ConfirmDialog
          title={translate("enrollments.enroll.outsidePackQuestion", {
            student: studentOutsidePack.fullName,
          })}
          message={translate("enrollments.enroll.outsidePackMessage", {
            classGroup: classGroupName,
          })}
          confirmLabel={translate("enrollments.enroll.confirmOutsidePack")}
          onCancel={() => setStudentOutsidePack(null)}
          onConfirm={() => enroll(studentOutsidePack, true)}
          isConfirming={enrollStudentMutation.isPending}
        />
      ) : null}
    </Screen>
  );
}
