import { useMutation, useQueryClient } from "@tanstack/react-query";
import { isApiError } from "@/api/httpClient";
import { notifyAbsence, withdrawAbsence } from "@/features/family/familyApi";
import { familyQueryKeys } from "@/features/family/familyQueryKeys";
import { FamilyNextClass } from "@/features/family/types";
import { translate } from "@/i18n/translate";
import { useToast } from "@/ui/ToastProvider";

const classStartedCode = "absence.class_started";

interface AbsenceVariables {
  studentId: string;
  nextClass: FamilyNextClass;
}

export function useAbsenceNotice() {
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const mutation = useMutation({
    mutationFn: ({ studentId, nextClass }: AbsenceVariables) =>
      nextClass.absenceNotified
        ? withdrawAbsence(studentId, nextClass.classGroupId ?? "", nextClass.date)
        : notifyAbsence(studentId, nextClass.classGroupId ?? "", nextClass.date),
    onSuccess: (_, { nextClass }) => {
      showToast(
        translate(
          nextClass.absenceNotified
            ? "family.absence.withdrawnToast"
            : "family.absence.notifiedToast",
        ),
      );
      return queryClient.invalidateQueries({ queryKey: familyQueryKeys.all });
    },
    onError: (error) =>
      showToast(
        translate(
          isApiError(error) && error.hasCode(classStartedCode)
            ? "family.absence.tooLate"
            : "common.unexpectedError",
        ),
      ),
  });
  return {
    toggleAbsence: (studentId: string, nextClass: FamilyNextClass) =>
      mutation.mutate({ studentId, nextClass }),
    isPending: mutation.isPending,
  };
}
