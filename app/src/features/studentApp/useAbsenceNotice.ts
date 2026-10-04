import { useMutation, useQueryClient } from "@tanstack/react-query";
import { isApiError } from "@/api/httpClient";
import { notifyAbsence, withdrawAbsence } from "@/features/studentApp/studentAppApi";
import { studentAppErrorCodes } from "@/features/studentApp/studentAppErrorCodes";
import { studentAppQueryKeys } from "@/features/studentApp/studentAppQueryKeys";
import { StudentAppNextClass } from "@/features/studentApp/types";
import { translate, TranslationKey } from "@/i18n/translate";
import { useToast } from "@/ui/ToastProvider";

function errorMessageOf(error: unknown): TranslationKey {
  if (isApiError(error) && error.hasCode(studentAppErrorCodes.classStarted)) {
    return "student.absence.tooLate";
  }
  return isApiError(error) && error.hasCode(studentAppErrorCodes.makeupCreditInUse)
    ? "student.absence.usedForMakeup"
    : "common.unexpectedError";
}

interface AbsenceVariables {
  studentId: string;
  nextClass: StudentAppNextClass;
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
            ? "student.absence.withdrawnToast"
            : "student.absence.notifiedToast",
        ),
      );
      return queryClient.invalidateQueries({ queryKey: studentAppQueryKeys.all });
    },
    onError: (error) => showToast(translate(errorMessageOf(error))),
  });
  return {
    toggleAbsence: (studentId: string, nextClass: StudentAppNextClass) =>
      mutation.mutate({ studentId, nextClass }),
    isPending: mutation.isPending,
  };
}
