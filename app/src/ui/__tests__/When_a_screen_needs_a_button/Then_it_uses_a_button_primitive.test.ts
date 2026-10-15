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
const primitivesDirectoryName = "ui";
const componentFileExtension = ".tsx";

const pressableFilesThatAreNotButtons: Readonly<Record<string, string>> = {
  "features/brand/components/BrandWelcome.tsx": "the whole welcome screen closes on a tap",
  "features/brand/components/ColorSwatchGrid.tsx": "color selector",
  "features/brand/components/ThemeOptionGrid.tsx": "theme selector",
  "features/catalogImages/components/CatalogImageField.tsx": "picks the main photo",
  "features/classGroups/components/ClassGroupPicker.tsx": "picker rows",
  "features/classPacks/components/ClassBalanceSection.tsx": "expandable pack group row",
  "features/classPacks/components/ClassPackPicker.tsx": "picker rows",
  "features/instructors/components/InstructorPicker.tsx": "picker rows",
  "features/sessions/components/AttendanceRow.tsx": "present and absent marks",
  "features/studentApp/components/PackCard.tsx": "pack card",
  "features/studentApp/components/ShopPreview.tsx": "product cards",
  "features/studentApp/components/ShopProductCard.tsx": "product card",
  "features/studentApp/components/StudentChips.tsx": "student selector",
  "features/students/components/StudentPicker.tsx": "picker rows",
  "forms/CalendarPicker.tsx": "calendar days",
  "forms/TimeColumn.tsx": "time options",
  "testing/SessionProbe.tsx": "test helper",
};

const pressablePattern = /\bPressable\b|\bPressableScale\b/;

function listComponentFiles(currentDirectory: string): string[] {
  return readdirSync(currentDirectory).flatMap((entryName) => {
    const entryPath = join(currentDirectory, entryName);
    if (statSync(entryPath).isDirectory()) {
      return entryName === testsDirectoryName ? [] : listComponentFiles(entryPath);
    }
    return entryName.endsWith(componentFileExtension) ? [entryPath] : [];
  });
}

describe("When a screen needs a button", () => {
  it("Then it uses a button primitive", () => {
    const filesOutsidePrimitives = listComponentFiles(sourceDirectory)
      .map((componentFile) => relative(sourceDirectory, componentFile).replace(/\\/g, "/"))
      .filter((componentFile) => !componentFile.startsWith(`${primitivesDirectoryName}/`));

    const handMadeButtonFiles = filesOutsidePrimitives.filter(
      (componentFile) =>
        pressablePattern.test(readFileSync(join(sourceDirectory, componentFile), "utf8")) &&
        pressableFilesThatAreNotButtons[componentFile] === undefined,
    );

    expect(filesOutsidePrimitives.length).toBeGreaterThan(0);
    expect(handMadeButtonFiles).toEqual([]);
  });
});
