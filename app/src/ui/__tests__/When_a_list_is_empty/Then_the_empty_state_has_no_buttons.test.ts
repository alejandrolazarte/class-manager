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

const emptyStateWithContentPattern = /<\/EmptyState>/;

describe("When a list is empty", () => {
  it("Then the empty state has no buttons", () => {
    const componentFiles = listComponentFiles(sourceDirectory);

    const emptyStatesWithContent = componentFiles
      .filter((componentFile) =>
        emptyStateWithContentPattern.test(readFileSync(componentFile, "utf8")),
      )
      .map((componentFile) => relative(sourceDirectory, componentFile));

    expect(componentFiles.length).toBeGreaterThan(0);
    expect(emptyStatesWithContent).toEqual([]);
  });
});
