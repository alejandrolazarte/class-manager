import { AuditLogExpectations } from "../support/auditLogRules";

export const auditLogExpectations: AuditLogExpectations = {
  expectedFailures: [
    {
      command: "SignInCommand",
      errorCode: "auth.invalid_credentials",
      reason: "When_signing_in_with_wrong_password",
    },
  ],
  commandsWithoutSession: [
    "RequestSignUpCommand",
    "CompleteSignUpCommand",
    "SignInCommand",
    "BeginPasskeySignInCommand",
    "SignInWithPasskeyCommand",
    "RefreshSessionCommand",
  ],
  expectedSecurityEvents: ["SignInFailed"],
};
