import { defineConfig, devices } from "@playwright/test";
import baseConfig from "./playwright.config";
import { e2eEnvironment } from "./support/environment";

const captureTimeoutMilliseconds = 180_000;
const smtpHost = "localhost";
const smtpFromAddress = "demo@e2e.local";

const baseWebServers = Array.isArray(baseConfig.webServer) ? baseConfig.webServer : [];
const [apiServer, ...otherServers] = baseWebServers;

export default defineConfig({
  ...baseConfig,
  testDir: "./screenshots",
  testMatch: "**/*.capture.ts",
  timeout: captureTimeoutMilliseconds,
  retries: 0,
  workers: 1,
  reporter: [["list"]],
  projects: [{ name: "mobile-chromium", use: { ...devices["Pixel 7"] } }],
  webServer:
    apiServer === undefined
      ? baseWebServers
      : [
          {
            ...apiServer,
            env: {
              ...apiServer.env,
              Email__Smtp__Host: smtpHost,
              Email__Smtp__Port: String(e2eEnvironment.smtpPort),
              Email__Smtp__FromAddress: smtpFromAddress,
            },
          },
          ...otherServers,
        ],
});
