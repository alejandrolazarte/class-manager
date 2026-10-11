import { defineConfig } from "@playwright/test";

const captureTimeoutMilliseconds = 300_000;

export default defineConfig({
  testDir: "./screenshots",
  testMatch: "Email_previews.capture.ts",
  timeout: captureTimeoutMilliseconds,
  retries: 0,
  workers: 1,
  reporter: [["list"]],
});
