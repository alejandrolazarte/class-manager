import {
  studentAppHiddenRouteNames,
  studentAppTabDefinitions,
} from "@/features/studentApp/studentAppTabDefinitions";
import { nodeFileSystem, nodePath } from "@/testing/nodeFileSystem";

const { readdirSync, statSync } = nodeFileSystem;
const { join } = nodePath;
const sourceDirectoryMarker = "/src/";
const routeFileExtension = ".tsx";
const layoutRouteName = "_layout";
const testPath = expect.getState().testPath ?? "";
const studentAppRoutesDirectory = join(
  testPath.slice(0, testPath.lastIndexOf(sourceDirectoryMarker)),
  "app",
  "student-app",
);

function listRouteNames(): string[] {
  return readdirSync(studentAppRoutesDirectory)
    .filter((entryName) => {
      const entryPath = join(studentAppRoutesDirectory, entryName);
      return statSync(entryPath).isDirectory() || entryName.endsWith(routeFileExtension);
    })
    .map((entryName) => entryName.replace(routeFileExtension, ""))
    .filter((routeName) => routeName !== layoutRouteName);
}

describe("When the student app routes are listed", () => {
  it("Then every route is a tab or hidden", () => {
    const knownRouteNames = [
      ...studentAppTabDefinitions.map((tab) => tab.name),
      ...studentAppHiddenRouteNames,
    ];

    const routesShownAsExtraTabs = listRouteNames().filter(
      (routeName) => !knownRouteNames.includes(routeName),
    );

    expect(routesShownAsExtraTabs).toEqual([]);
  });
});
