import { bookPackClass, cancelPackClass } from "@/features/family/familyApi";
import { familyErrorCodes } from "@/features/family/familyErrorCodes";
import { SlotBookingVariables, useSlotBooking } from "@/features/family/useSlotBooking";

export function usePackClassBooking() {
  const { toggle, pendingKey } = useSlotBooking({
    book: bookPackClass,
    cancel: cancelPackClass,
    bookedToastKey: "family.packClasses.bookedToast",
    cancelledToastKey: "family.packClasses.cancelledToast",
    errorMessages: [
      [familyErrorCodes.classStarted, "family.absence.tooLate"],
      [familyErrorCodes.makeupFull, "family.makeup.full"],
      [familyErrorCodes.packNoClasses, "family.packClasses.noClasses"],
    ],
  });
  return {
    togglePackClass: (variables: SlotBookingVariables) => toggle(variables),
    pendingKey,
  };
}
