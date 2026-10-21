const { spawnSync } = require("node:child_process");
const { applyBackofficeWebIdentity } = require("./applyBackofficeWebIdentity");

const outputDirectory = "dist-backoffice";
const backofficeEnvironment = { ...process.env, APP_TARGET: "backoffice" };

function run(command, args, environment = process.env) {
  const result = spawnSync(command, args, {
    stdio: "inherit",
    env: environment,
    shell: process.platform === "win32",
  });
  if (result.status !== 0) {
    process.exit(result.status ?? 1);
  }
}

run(
  "pnpm",
  ["exec", "expo", "export", "--platform", "web", "--output-dir", outputDirectory],
  backofficeEnvironment,
);
run("node", ["scripts/relocateNodeModulesAssets.js", outputDirectory]);
run("node", ["scripts/writeStaticWebAppConfig.js", outputDirectory]);
applyBackofficeWebIdentity(outputDirectory);
