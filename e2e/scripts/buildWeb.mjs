import { spawnSync } from "node:child_process";
import { fileURLToPath } from "node:url";

const appDirectory = fileURLToPath(new URL("../../app", import.meta.url));
const outputDirectory = fileURLToPath(new URL("../.web-build", import.meta.url));
const apiUrl = process.env.E2E_API_URL ?? "http://localhost:5000";

const result = spawnSync(
  "pnpm",
  ["exec", "expo", "export", "--platform", "web", "--output-dir", outputDirectory, "--clear"],
  {
    cwd: appDirectory,
    stdio: "inherit",
    env: { ...process.env, EXPO_PUBLIC_API_BASE_URL: apiUrl },
  },
);
if (result.status !== 0) {
  process.exit(result.status ?? 1);
}

const relocation = spawnSync("node", ["scripts/relocateNodeModulesAssets.js", outputDirectory], {
  cwd: appDirectory,
  stdio: "inherit",
});
process.exit(relocation.status ?? 1);
