import { nodeFileSystem, nodePath } from "@/testing/nodeFileSystem";

const { readFileSync, readdirSync, statSync } = nodeFileSystem;
const { join, relative } = nodePath;
const sourceDirectoryMarker = "/src/";
const testPath = expect.getState().testPath ?? "";
const sourceDirectory = testPath.slice(
  0,
  testPath.lastIndexOf(sourceDirectoryMarker) + sourceDirectoryMarker.length,
);
const testsDirectoryName = "__tests__";
const componentFileExtension = ".tsx";

function listComponentFiles(currentDirectory: string): string[] {
  return readdirSync(currentDirectory).flatMap((entryName) => {
    const entryPath = join(currentDirectory, entryName);
    if (statSync(entryPath).isDirectory()) {
      return entryName === testsDirectoryName ? [] : listComponentFiles(entryPath);
    }
    return entryName.endsWith(componentFileExtension) ? [entryPath] : [];
  });
}

const floatingButtonPattern = /<FloatingActionButton\b/;
const floatingButtonHiddenWhenEmptyPattern =
  /(\.length\s*(>\s*0|!==\s*0|>=\s*1)|\.length\s*&&)[^?]*\?\s*\(?\s*<FloatingActionButton\b/;

describe("When a list is empty", () => {
  it("Then the floating button stays visible", () => {
    const floatingButtonFiles = listComponentFiles(sourceDirectory).filter((componentFile) =>
      floatingButtonPattern.test(readFileSync(componentFile, "utf8")),
    );

    const floatingButtonsHiddenWhenEmpty = floatingButtonFiles
      .filter((componentFile) =>
        floatingButtonHiddenWhenEmptyPattern.test(readFileSync(componentFile, "utf8")),
      )
      .map((componentFile) => relative(sourceDirectory, componentFile));

    expect(floatingButtonFiles.length).toBeGreaterThan(0);
    expect(floatingButtonsHiddenWhenEmpty).toEqual([]);
  });
});
