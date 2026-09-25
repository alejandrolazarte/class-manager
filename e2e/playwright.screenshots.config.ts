import { defineConfig, devices } from "@playwright/test";
import baseConfig from "./playwright.config";

const captureTimeoutMilliseconds = 180_000;

export default defineConfig({
  ...baseConfig,
  testDir: "./screenshots",
  testMatch: "**/*.capture.ts",
  timeout: captureTimeoutMilliseconds,
  retries: 0,
  workers: 1,
  reporter: [["list"]],
  projects: [{ name: "mobile-chromium", use: { ...devices["Pixel 7"] } }],
});
