import { useMutation, useQueryClient } from "@tanstack/react-query";
import { isApiError } from "@/api/httpClient";
import { notifyAbsence, withdrawAbsence } from "@/features/family/familyApi";
import { familyErrorCodes } from "@/features/family/familyErrorCodes";
import { familyQueryKeys } from "@/features/family/familyQueryKeys";
import { FamilyNextClass } from "@/features/family/types";
import { translate, TranslationKey } from "@/i18n/translate";
import { useToast } from "@/ui/ToastProvider";

function errorMessageOf(error: unknown): TranslationKey {
  if (isApiError(error) && error.hasCode(familyErrorCodes.classStarted)) {
    return "family.absence.tooLate";
  }
  return isApiError(error) && error.hasCode(familyErrorCodes.makeupCreditInUse)
    ? "family.absence.usedForMakeup"
    : "common.unexpectedError";
}

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
    onError: (error) => showToast(translate(errorMessageOf(error))),
  });
  return {
    toggleAbsence: (studentId: string, nextClass: FamilyNextClass) =>
      mutation.mutate({ studentId, nextClass }),
    isPending: mutation.isPending,
  };
}
