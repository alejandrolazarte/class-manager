import { spawnSync } from "node:child_process";
import { fileURLToPath } from "node:url";

const repositoryRoot = fileURLToPath(new URL("../..", import.meta.url));
const connectionString = process.env.E2E_DATABASE_CONNECTION_STRING;
const migrations = [
  { context: "AppDbContext", project: "src/Infrastructure" },
  { context: "SecurityDbContext", project: "src/Security" },
];

if (!connectionString) {
  console.error("Set E2E_DATABASE_CONNECTION_STRING to a dedicated e2e database.");
  process.exit(1);
}

function run(arguments_) {
  const result = spawnSync("dotnet", arguments_, { cwd: repositoryRoot, stdio: "inherit" });
  if (result.status !== 0) {
    process.exit(result.status ?? 1);
  }
}

run(["tool", "restore"]);
for (const { context, project } of migrations) {
  run([
    "ef",
    "database",
    "update",
    "--project",
    project,
    "--startup-project",
    "src/Api",
    "--context",
    context,
    "--connection",
    connectionString,
  ]);
}
