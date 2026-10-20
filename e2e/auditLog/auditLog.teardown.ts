import { expect, test as teardown } from "@playwright/test";
import { auditLogRunStartedAt, readAuditLogOnceSettled } from "../support/auditLog";
import { auditLogViolations } from "../support/auditLogRules";
import { rememberedPersonalData } from "../support/personalData";
import { auditLogExpectations } from "./auditLogExpectations";

teardown("The audit log follows the rules", async () => {
  const auditLog = await readAuditLogOnceSettled(auditLogRunStartedAt());

  const violations = auditLogViolations(auditLog, rememberedPersonalData(), auditLogExpectations);

  expect(violations).toEqual([]);
});
