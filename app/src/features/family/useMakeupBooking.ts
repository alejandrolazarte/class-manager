import { useMutation, useQueryClient } from "@tanstack/react-query";
import { isApiError } from "@/api/httpClient";
import { bookMakeup, cancelMakeup } from "@/features/family/familyApi";
import { familyErrorCodes } from "@/features/family/familyErrorCodes";
import { familyQueryKeys } from "@/features/family/familyQueryKeys";
import { translate, TranslationKey } from "@/i18n/translate";
import { useToast } from "@/ui/ToastProvider";

interface MakeupVariables {
  studentId: string;
  classGroupId: string;
  date: string;
  isBooked: boolean;
}

const errorMessages: [code: string, message: TranslationKey][] = [
  [familyErrorCodes.classStarted, "family.absence.tooLate"],
  [familyErrorCodes.makeupFull, "family.makeup.full"],
  [familyErrorCodes.makeupNoCredit, "family.makeup.noCredit"],
];

function errorMessageOf(error: unknown): TranslationKey {
  const match = errorMessages.find(([code]) => isApiError(error) && error.hasCode(code));
  return match?.[1] ?? "common.unexpectedError";
}

export function useMakeupBooking() {
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const mutation = useMutation({
    mutationFn: ({ studentId, classGroupId, date, isBooked }: MakeupVariables) =>
      isBooked
        ? cancelMakeup(studentId, classGroupId, date)
        : bookMakeup(studentId, classGroupId, date),
    onSuccess: (_, { isBooked }) => {
      showToast(translate(isBooked ? "family.makeup.cancelledToast" : "family.makeup.bookedToast"));
      return queryClient.invalidateQueries({ queryKey: familyQueryKeys.all });
    },
    onError: (error) => showToast(translate(errorMessageOf(error))),
  });
  return {
    toggleMakeup: (variables: MakeupVariables) => mutation.mutate(variables),
    pendingKey: mutation.isPending
      ? `${mutation.variables?.classGroupId}-${mutation.variables?.date}`
      : null,
  };
}
