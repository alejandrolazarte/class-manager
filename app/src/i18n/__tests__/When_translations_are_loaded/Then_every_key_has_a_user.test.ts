import spanishTranslations from "@/i18n/es.json";
import { nodeFileSystem, nodePath } from "@/testing/nodeFileSystem";

const { readFileSync, readdirSync, statSync } = nodeFileSystem;
const { join } = nodePath;
const sourceDirectoryMarker = "/src/";
const testPath = expect.getState().testPath ?? "";
const sourceDirectory = testPath.slice(
  0,
  testPath.lastIndexOf(sourceDirectoryMarker) + sourceDirectoryMarker.length,
);
const routesDirectory = join(sourceDirectory, "..", "app");
const skippedDirectoryNames = ["__tests__", "testing"];
const codeFileExtensions = [".ts", ".tsx"];
const keySegmentSeparator = ".";
const pluralFormPattern = /\.(one|other)$/;
const quotes = ['"', "'", "`"];

const keysBuiltFromOneValue = [
  "countries.",
  "fees.billingPlan.hint.",
  "fees.methods.",
  "fees.plan.",
  "instructors.app.",
  "months.",
  "orders.counterSale.filter.",
  "orders.emptyFilter.",
  "orders.filter.",
  "orders.status.",
  "products.stock.kind.",
  "products.stockMode.",
  "products.stockModeHint.",
  "roles.",
  "student.app.",
  "student.medals.",
  "subscriptions.feature.",
  "subscriptions.plan.",
  "themes.",
  "weekdays.long.",
  "weekdays.short.",
];

const keysBuiltFromApiErrorCodes = [
  "apiErrors.",
  "importExport.fileError.",
  "importExport.rowError.",
];

function listCodeFiles(currentDirectory: string): string[] {
  return readdirSync(currentDirectory).flatMap((entryName) => {
    const entryPath = join(currentDirectory, entryName);
    if (statSync(entryPath).isDirectory()) {
      return skippedDirectoryNames.includes(entryName) ? [] : listCodeFiles(entryPath);
    }
    return codeFileExtensions.some((extension) => entryName.endsWith(extension)) ? [entryPath] : [];
  });
}

function isBuiltAtRuntime(key: string): boolean {
  const isBuiltFromOneValue = keysBuiltFromOneValue.some(
    (prefix) => key.startsWith(prefix) && !key.slice(prefix.length).includes(keySegmentSeparator),
  );
  return isBuiltFromOneValue || keysBuiltFromApiErrorCodes.some((prefix) => key.startsWith(prefix));
}

function isWrittenIn(code: string, key: string): boolean {
  const pluralBaseKey = key.replace(pluralFormPattern, "");
  return quotes.some(
    (quote) =>
      code.includes(`${quote}${key}${quote}`) || code.includes(`${quote}${pluralBaseKey}${quote}`),
  );
}

describe("When translations are loaded", () => {
  it("Then every key has a user", () => {
    const code = [...listCodeFiles(sourceDirectory), ...listCodeFiles(routesDirectory)]
      .map((codeFile) => readFileSync(codeFile, "utf8"))
      .join("\n");

    const keysWithoutUsers = Object.keys(spanishTranslations).filter(
      (key) => !isBuiltAtRuntime(key) && !isWrittenIn(code, key),
    );

    expect(keysWithoutUsers).toEqual([]);
  });
});
