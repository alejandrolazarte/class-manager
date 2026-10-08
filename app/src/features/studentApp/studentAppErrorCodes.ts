export const studentAppErrorCodes = {
  invalidInvitation: "student_invitation.invalid",
  alreadyLinked: "student.already_linked",
  emailRequired: "student_invitation.email_required",
  guardianEmailRequired: "student_invitation.guardian_email_required",
  outOfStock: "product.out_of_stock",
  tooManyOpenOrders: "order.too_many_open",
  classStarted: "absence.class_started",
  attendanceTaken: "absence.attendance_taken",
  makeupNoCredit: "makeup.no_credit",
  makeupFull: "makeup.full",
  makeupCreditInUse: "makeup.credit_in_use",
  packNoClasses: "pack_booking.no_classes",
} as const;
