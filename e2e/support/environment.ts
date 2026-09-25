import { randomBytes } from "node:crypto";

const jwtSigningKeyBytes = 48;
const missingConnectionStringMessage =
  "Set E2E_DATABASE_CONNECTION_STRING to a dedicated e2e database (see docs/e2e/running-e2e-tests.md).";

export const e2eEnvironment = {
  isContinuousIntegration: process.env.CI !== undefined,
  apiUrl: process.env.E2E_API_URL ?? "http://localhost:5000",
  webUrl: process.env.E2E_WEB_URL ?? "http://localhost:8081",
  webPort: process.env.E2E_WEB_PORT ?? "8081",
  jwtSigningKey:
    process.env.E2E_JWT_SIGNING_KEY ?? randomBytes(jwtSigningKeyBytes).toString("base64"),
  authenticationPermitLimit: "10000",
};

export function requireDatabaseConnectionString(): string {
  const connectionString = process.env.E2E_DATABASE_CONNECTION_STRING;
  if (connectionString === undefined || connectionString === "") {
    throw new Error(missingConnectionStringMessage);
  }
  return connectionString;
}
