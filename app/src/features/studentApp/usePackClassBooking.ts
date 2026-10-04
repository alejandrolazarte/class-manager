import { bookPackClass, cancelPackClass } from "@/features/studentApp/studentAppApi";
import { studentAppErrorCodes } from "@/features/studentApp/studentAppErrorCodes";
import { SlotBookingVariables, useSlotBooking } from "@/features/studentApp/useSlotBooking";

export function usePackClassBooking() {
  const { toggle, pendingKey } = useSlotBooking({
    book: bookPackClass,
    cancel: cancelPackClass,
    bookedToastKey: "student.packClasses.bookedToast",
    cancelledToastKey: "student.packClasses.cancelledToast",
    errorMessages: [
      [studentAppErrorCodes.classStarted, "student.absence.tooLate"],
      [studentAppErrorCodes.makeupFull, "student.makeup.full"],
      [studentAppErrorCodes.packNoClasses, "student.packClasses.noClasses"],
    ],
  });
  return {
    togglePackClass: (variables: SlotBookingVariables) => toggle(variables),
    pendingKey,
  };
}
