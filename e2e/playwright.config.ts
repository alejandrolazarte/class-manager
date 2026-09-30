import { defineConfig, devices } from "@playwright/test";
import { e2eEnvironment, requireDatabaseConnectionString } from "./support/environment";

const apiStartTimeoutMilliseconds = 180_000;
const webStartTimeoutMilliseconds = 30_000;
const testTimeoutMilliseconds = 60_000;
const businessTimeZoneId = "America/Argentina/Buenos_Aires";

export default defineConfig({
  testDir: "./tests",
  testMatch: "**/*.spec.ts",
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
  projects: [{ name: "mobile-chromium", use: { ...devices["Pixel 7"] } }],
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
        Authentication__Jwt__SigningKey: e2eEnvironment.jwtSigningKey,
        Authentication__RateLimit__PermitLimit: e2eEnvironment.authenticationPermitLimit,
        Cors__AllowedOrigins__0: e2eEnvironment.webUrl,
        WebPush__Vapid__Subject: e2eEnvironment.vapidSubject,
        WebPush__Vapid__PublicKey: e2eEnvironment.vapidKeys.publicKey,
        WebPush__Vapid__PrivateKey: e2eEnvironment.vapidKeys.privateKey,
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
