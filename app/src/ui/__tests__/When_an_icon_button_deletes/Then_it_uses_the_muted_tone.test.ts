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
const deleteIconAttribute = 'icon="delete"';
const mutedToneAttribute = 'tone="muted-foreground"';

const iconButtonPattern = /<IconButton\b[\s\S]*?\/>/g;

function listComponentFiles(currentDirectory: string): string[] {
  return readdirSync(currentDirectory).flatMap((entryName) => {
    const entryPath = join(currentDirectory, entryName);
    if (statSync(entryPath).isDirectory()) {
      return entryName === testsDirectoryName ? [] : listComponentFiles(entryPath);
    }
    return entryName.endsWith(componentFileExtension) ? [entryPath] : [];
  });
}

function deleteIconButtonsOf(componentFile: string): string[] {
  const source = readFileSync(join(sourceDirectory, componentFile), "utf8");
  return (source.match(iconButtonPattern) ?? []).filter((iconButton) =>
    iconButton.includes(deleteIconAttribute),
  );
}

describe("When an icon button deletes", () => {
  it("Then it uses the muted tone", () => {
    const deleteIconButtons = listComponentFiles(sourceDirectory)
      .map((componentFile) => relative(sourceDirectory, componentFile).replace(/\\/g, "/"))
      .flatMap((componentFile) =>
        deleteIconButtonsOf(componentFile).map((iconButton) => ({ componentFile, iconButton })),
      );

    const filesWithAnotherTone = deleteIconButtons
      .filter(({ iconButton }) => !iconButton.includes(mutedToneAttribute))
      .map(({ componentFile }) => componentFile);

    expect(deleteIconButtons.length).toBeGreaterThan(0);
    expect(filesWithAnotherTone).toEqual([]);
  });
});
