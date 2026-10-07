export const authenticationErrorCodes = {
  emailTaken: "auth.email_taken",
  invalidCredentials: "auth.invalid_credentials",
  lockedOut: "auth.locked_out",
  invalidRefreshToken: "auth.invalid_refresh_token",
  invalidPasswordResetToken: "auth.invalid_password_reset_token",
  tooYoungForOwnAccount: "auth.too_young_for_own_account",
  ownerMustBeAdult: "auth.owner_must_be_adult",
} as const;
