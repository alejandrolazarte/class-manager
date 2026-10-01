import { nodeFileSystem, nodePath } from "@/testing/nodeFileSystem";

const { readFileSync, readdirSync, statSync } = nodeFileSystem;
const { join, relative } = nodePath;
const sourceDirectoryMarker = "/src/";
const testPath = expect.getState().testPath ?? "";
const sourceDirectory = testPath.slice(
  0,
  testPath.lastIndexOf(sourceDirectoryMarker) + sourceDirectoryMarker.length,
);
const featuresDirectory = join(sourceDirectory, "features");
const screensDirectoryName = "screens";
const screenFileExtension = ".tsx";
const formScreenPattern = /\buseForm\(|<SettingsFormScreenLayout\b/;
const backHeaderPattern = /navigation="back"/;
const itemStateWithoutClosePattern = /<SettingsItemState(?![^>]*navigation="close")[^>]*>/;

function listScreenFiles(currentDirectory: string): string[] {
  return readdirSync(currentDirectory).flatMap((entryName) => {
    const entryPath = join(currentDirectory, entryName);
    if (statSync(entryPath).isDirectory()) {
      return listScreenFiles(entryPath);
    }
    const isScreen =
      entryName.endsWith(screenFileExtension) && currentDirectory.endsWith(screensDirectoryName);
    return isScreen ? [entryPath] : [];
  });
}

describe("When a screen edits a form", () => {
  it("Then its header closes instead of going back", () => {
    const formScreens = listScreenFiles(featuresDirectory).filter((screenFile) =>
      formScreenPattern.test(readFileSync(screenFile, "utf8")),
    );

    const formScreensGoingBack = formScreens
      .filter((screenFile) => {
        const sourceText = readFileSync(screenFile, "utf8");
        return backHeaderPattern.test(sourceText) || itemStateWithoutClosePattern.test(sourceText);
      })
      .map((screenFile) => relative(sourceDirectory, screenFile));

    expect(formScreens.length).toBeGreaterThan(0);
    expect(formScreensGoingBack).toEqual([]);
  });
});
