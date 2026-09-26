import { spawnSync } from "node:child_process";
import { fileURLToPath } from "node:url";

const connectionStringVariable = "MIGRATIONS_CONNECTION_STRING";
const repositoryRoot = fileURLToPath(new URL("..", import.meta.url));
const connectionString = process.env[connectionStringVariable];
const migrations = [
  { context: "AppDbContext", project: "src/Infrastructure" },
  { context: "SecurityDbContext", project: "src/Security" },
];

if (!connectionString) {
  console.error(
    `Set ${connectionStringVariable} to the connection string of the database to migrate.`,
  );
  process.exit(1);
}

function run(dotnetArguments) {
  const result = spawnSync("dotnet", dotnetArguments, {
    cwd: repositoryRoot,
    stdio: "inherit",
  });
  if (result.status !== 0) {
    process.exit(result.status ?? 1);
  }
}

run(["tool", "restore"]);
run(["restore", "src/Api/Api.csproj"]);
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
