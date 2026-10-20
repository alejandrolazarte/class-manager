import { appendFileSync, readdirSync, readFileSync } from "node:fs";
import { join } from "node:path";
import { e2eEnvironment } from "./environment";

const valueSeparator = "\n";

export function personal<TValue extends string>(value: TValue): TValue {
  appendFileSync(
    join(e2eEnvironment.personalDataDirectory, `${process.pid}.txt`),
    value + valueSeparator,
  );
  return value;
}

export function rememberedPersonalData(): string[] {
  const values = readdirSync(e2eEnvironment.personalDataDirectory).flatMap((fileName) =>
    readFileSync(join(e2eEnvironment.personalDataDirectory, fileName), "utf8").split(
      valueSeparator,
    ),
  );
  return [...new Set(values.filter((value) => value !== ""))];
}
