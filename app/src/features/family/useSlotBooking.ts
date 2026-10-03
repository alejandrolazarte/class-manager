import { useMutation, useQueryClient } from "@tanstack/react-query";
import { isApiError } from "@/api/httpClient";
import { familyQueryKeys } from "@/features/family/familyQueryKeys";
import { translate, TranslationKey } from "@/i18n/translate";
import { useToast } from "@/ui/ToastProvider";

export interface SlotBookingVariables {
  studentId: string;
  classGroupId: string;
  date: string;
  isBooked: boolean;
}

type SlotRequest = (studentId: string, classGroupId: string, date: string) => Promise<void>;

export interface SlotBookingOptions {
  book: SlotRequest;
  cancel: SlotRequest;
  bookedToastKey: TranslationKey;
  cancelledToastKey: TranslationKey;
  errorMessages: [code: string, message: TranslationKey][];
}

export function useSlotBooking({
  book,
  cancel,
  bookedToastKey,
  cancelledToastKey,
  errorMessages,
}: SlotBookingOptions) {
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const errorMessageOf = (error: unknown): TranslationKey =>
    errorMessages.find(([code]) => isApiError(error) && error.hasCode(code))?.[1] ??
    "common.unexpectedError";
  const mutation = useMutation({
    mutationFn: ({ studentId, classGroupId, date, isBooked }: SlotBookingVariables) =>
      isBooked ? cancel(studentId, classGroupId, date) : book(studentId, classGroupId, date),
    onSuccess: (_, { isBooked }) => {
      showToast(translate(isBooked ? cancelledToastKey : bookedToastKey));
      return queryClient.invalidateQueries({ queryKey: familyQueryKeys.all });
    },
    onError: (error) => showToast(translate(errorMessageOf(error))),
  });
  return {
    toggle: (variables: SlotBookingVariables) => mutation.mutate(variables),
    pendingKey: mutation.isPending
      ? `${mutation.variables?.classGroupId}-${mutation.variables?.date}`
      : null,
  };
}
