import { defineConfig, devices } from "@playwright/test";
import { e2eEnvironment, requireDatabaseConnectionString } from "./support/environment";

const apiStartTimeoutMilliseconds = 180_000;
const webStartTimeoutMilliseconds = 30_000;
const testTimeoutMilliseconds = 60_000;
const businessTimeZoneId = "America/Argentina/Buenos_Aires";

export default defineConfig({
  testDir: "./tests",
  testMatch: "**/*.spec.ts",
  globalSetup: "./support/globalSetup.ts",
  timeout: testTimeoutMilliseconds,
  forbidOnly: e2eEnvironment.isContinuousIntegration,
  retries: e2eEnvironment.isContinuousIntegration ? 1 : 0,
  workers: 2,
  reporter: e2eEnvironment.isContinuousIntegration
    ? [["github"], ["html", { open: "never" }]]
    : [["list"], ["html", { open: "never" }]],
  use: {
    baseURL: e2eEnvironment.webUrl,
    locale: "es-AR",
    timezoneId: businessTimeZoneId,
    trace: "retain-on-failure",
    screenshot: "only-on-failure",
  },
  projects: [
    {
      name: "audit-log",
      testDir: "./auditLog",
      testMatch: "auditLog.setup.ts",
      teardown: "audit-log-rules",
    },
    {
      name: "audit-log-rules",
      testDir: "./auditLog",
      testMatch: "auditLog.teardown.ts",
    },
    {
      name: "mobile-chromium",
      use: { ...devices["Pixel 7"] },
      dependencies: ["audit-log"],
    },
  ],
  webServer: [
    {
      command: "dotnet run --project ../src/Api/Api.csproj --no-launch-profile",
      url: `${e2eEnvironment.apiUrl}/health`,
      timeout: apiStartTimeoutMilliseconds,
      reuseExistingServer: !e2eEnvironment.isContinuousIntegration,
      env: {
        ASPNETCORE_ENVIRONMENT: "Development",
        ASPNETCORE_URLS: e2eEnvironment.apiUrl,
        ConnectionStrings__BusinessDatabase: requireDatabaseConnectionString(),
        ConnectionStrings__AuditLog: e2eEnvironment.auditLogConnectionString,
        Subscriptions__SignUpPlan: "enterprise",
        Authentication__Jwt__SigningKey: e2eEnvironment.jwtSigningKey,
        Authentication__RateLimit__PermitLimit: e2eEnvironment.authenticationPermitLimit,
        Cors__AllowedOrigins__0: e2eEnvironment.webUrl,
        WebPush__Vapid__Subject: e2eEnvironment.vapidSubject,
        WebPush__Vapid__PublicKey: e2eEnvironment.vapidKeys.publicKey,
        WebPush__Vapid__PrivateKey: e2eEnvironment.vapidKeys.privateKey,
        Email__Smtp__Host: e2eEnvironment.smtpHost,
        Email__Smtp__Port: String(e2eEnvironment.smtpPort),
        Email__Smtp__FromAddress: e2eEnvironment.smtpFromAddress,
      },
    },
    {
      command: `pnpm exec serve --single --no-clipboard --listen ${e2eEnvironment.webPort} .web-build`,
      url: e2eEnvironment.webUrl,
      timeout: webStartTimeoutMilliseconds,
      reuseExistingServer: !e2eEnvironment.isContinuousIntegration,
    },
  ],
});
