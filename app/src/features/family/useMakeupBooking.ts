import { bookMakeup, cancelMakeup } from "@/features/family/familyApi";
import { familyErrorCodes } from "@/features/family/familyErrorCodes";
import { SlotBookingVariables, useSlotBooking } from "@/features/family/useSlotBooking";

export function useMakeupBooking() {
  const { toggle, pendingKey } = useSlotBooking({
    book: bookMakeup,
    cancel: cancelMakeup,
    bookedToastKey: "family.makeup.bookedToast",
    cancelledToastKey: "family.makeup.cancelledToast",
    errorMessages: [
      [familyErrorCodes.classStarted, "family.absence.tooLate"],
      [familyErrorCodes.makeupFull, "family.makeup.full"],
      [familyErrorCodes.makeupNoCredit, "family.makeup.noCredit"],
    ],
  });
  return {
    toggleMakeup: (variables: SlotBookingVariables) => toggle(variables),
    pendingKey,
  };
}
