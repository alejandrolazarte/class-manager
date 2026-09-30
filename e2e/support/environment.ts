import { generateKeyPairSync, randomBytes } from "node:crypto";

const jwtSigningKeyBytes = 48;
const uncompressedPointPrefix = 0x04;
const vapidSubject = "mailto:e2e@example.com";

function generateVapidKeys(): { publicKey: string; privateKey: string } {
  const jwk = generateKeyPairSync("ec", { namedCurve: "prime256v1" }).privateKey.export({
    format: "jwk",
  });
  const publicKey = Buffer.concat([
    Buffer.from([uncompressedPointPrefix]),
    Buffer.from(jwk.x!, "base64url"),
    Buffer.from(jwk.y!, "base64url"),
  ]).toString("base64url");
  return { publicKey, privateKey: jwk.d! };
}
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
  smtpPort: Number(process.env.E2E_SMTP_PORT ?? "2525"),
  vapidSubject,
  vapidKeys: generateVapidKeys(),
};

export function requireDatabaseConnectionString(): string {
  const connectionString = process.env.E2E_DATABASE_CONNECTION_STRING;
  if (connectionString === undefined || connectionString === "") {
    throw new Error(missingConnectionStringMessage);
  }
  return connectionString;
}
