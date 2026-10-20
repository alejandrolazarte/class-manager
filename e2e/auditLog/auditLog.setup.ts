import { test as setup } from "@playwright/test";
import { listCommandTables } from "../support/auditLog";

const azuriteUnreachableMessage =
  "Azurite is not reachable: start it before the e2e tests (see docs/e2e/running-e2e-tests.md).";

setup("Azurite is reachable", async () => {
  try {
    await listCommandTables();
  } catch (error) {
    throw new Error(azuriteUnreachableMessage, { cause: error });
  }
});
