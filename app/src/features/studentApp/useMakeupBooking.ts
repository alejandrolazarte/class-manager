import { bookMakeup, cancelMakeup } from "@/features/studentApp/studentAppApi";
import { studentAppErrorCodes } from "@/features/studentApp/studentAppErrorCodes";
import { SlotBookingVariables, useSlotBooking } from "@/features/studentApp/useSlotBooking";

export function useMakeupBooking() {
  const { toggle, pendingKey } = useSlotBooking({
    book: bookMakeup,
    cancel: cancelMakeup,
    bookedToastKey: "student.makeup.bookedToast",
    cancelledToastKey: "student.makeup.cancelledToast",
    errorMessages: [
      [studentAppErrorCodes.classStarted, "student.absence.tooLate"],
      [studentAppErrorCodes.makeupFull, "student.makeup.full"],
      [studentAppErrorCodes.makeupNoCredit, "student.makeup.noCredit"],
    ],
  });
  return {
    toggleMakeup: (variables: SlotBookingVariables) => toggle(variables),
    pendingKey,
  };
}
