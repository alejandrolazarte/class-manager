import { spawnSync } from "node:child_process";
import { fileURLToPath } from "node:url";

const migrationScript = fileURLToPath(
  new URL("../../scripts/migrate-database.mjs", import.meta.url),
);
const connectionString = process.env.E2E_DATABASE_CONNECTION_STRING;

if (!connectionString) {
  console.error("Set E2E_DATABASE_CONNECTION_STRING to a dedicated e2e database.");
  process.exit(1);
}

const result = spawnSync(process.execPath, [migrationScript], {
  stdio: "inherit",
  env: { ...process.env, MIGRATIONS_CONNECTION_STRING: connectionString },
});
process.exit(result.status ?? 1);
