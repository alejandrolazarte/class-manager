import { spawnSync } from "node:child_process";
import { fileURLToPath } from "node:url";

const connectionStringVariable = "MIGRATIONS_CONNECTION_STRING";
const maximumMigrationAttempts = 6;
const secondsBetweenMigrationAttempts = 30;
const millisecondsPerSecond = 1000;
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

function runDotnet(dotnetArguments) {
  return spawnSync("dotnet", dotnetArguments, {
    cwd: repositoryRoot,
    stdio: "inherit",
  }).status;
}

function run(dotnetArguments) {
  const status = runDotnet(dotnetArguments);
  if (status !== 0) {
    process.exit(status ?? 1);
  }
}

function waitSeconds(seconds) {
  Atomics.wait(
    new Int32Array(new SharedArrayBuffer(Int32Array.BYTES_PER_ELEMENT)),
    0,
    0,
    seconds * millisecondsPerSecond,
  );
}

function runWhileTheDatabaseResumes(dotnetArguments) {
  for (let attempt = 1; ; attempt++) {
    const status = runDotnet(dotnetArguments);
    if (status === 0) {
      return;
    }
    if (attempt === maximumMigrationAttempts) {
      process.exit(status ?? 1);
    }
    console.warn(
      `Attempt ${attempt} of ${maximumMigrationAttempts} failed. A paused database takes about a minute to resume; retrying in ${secondsBetweenMigrationAttempts} seconds.`,
    );
    waitSeconds(secondsBetweenMigrationAttempts);
  }
}

run(["tool", "restore"]);
run(["restore", "src/Api/Api.csproj"]);
for (const { context, project } of migrations) {
  runWhileTheDatabaseResumes([
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
