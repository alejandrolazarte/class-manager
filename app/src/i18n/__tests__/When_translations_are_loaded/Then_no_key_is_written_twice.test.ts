import { nodeFileSystem, nodePath } from "@/testing/nodeFileSystem";

const { readFileSync } = nodeFileSystem;
const { dirname, join } = nodePath;
const translationFileNames = ["es.json", "en.json"];
const keyLinePattern = /^\s*"([^"]+)":/gm;
const testPath = expect.getState().testPath ?? "";
const translationsDirectory = join(dirname(testPath), "..", "..");

function repeatedKeysOf(fileName: string): string[] {
  const content = readFileSync(join(translationsDirectory, fileName), "utf8");
  const keys = [...content.matchAll(keyLinePattern)].map((match) => match[1] ?? "");
  return keys.filter((key, index) => keys.indexOf(key) !== index);
}

describe("When translations are loaded", () => {
  it("Then no key is written twice", () => {
    expect(
      translationFileNames.map((fileName) => ({ fileName, keys: repeatedKeysOf(fileName) })),
    ).toEqual(translationFileNames.map((fileName) => ({ fileName, keys: [] })));
  });
});
