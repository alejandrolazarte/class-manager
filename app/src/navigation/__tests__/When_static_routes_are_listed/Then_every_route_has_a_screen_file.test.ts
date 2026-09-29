import { routes } from "@/navigation/routes";
import { nodeFileSystem, nodePath } from "@/testing/nodeFileSystem";

const { readdirSync, statSync } = nodeFileSystem;
const { join, relative } = nodePath;
const appRootFromTestFile = "../../../../..";
const routerDirectory = join(expect.getState().testPath ?? "", appRootFromTestFile, "app");
const routeFileExtension = ".tsx";
const layoutFileName = "_layout.tsx";
const indexSegment = "index";
const groupSegmentPattern = /^\(.+\)$/;

function listRouteFiles(currentDirectory: string): string[] {
  return readdirSync(currentDirectory).flatMap((entryName) => {
    const entryPath = join(currentDirectory, entryName);
    if (statSync(entryPath).isDirectory()) {
      return listRouteFiles(entryPath);
    }
    return entryName.endsWith(routeFileExtension) && entryName !== layoutFileName
      ? [relative(routerDirectory, entryPath)]
      : [];
  });
}

function toUrlPath(routeFile: string): string {
  const segments = routeFile
    .slice(0, -routeFileExtension.length)
    .split("/")
    .filter((segment) => !groupSegmentPattern.test(segment));
  const withoutIndex = segments.at(-1) === indexSegment ? segments.slice(0, -1) : segments;
  return `/${withoutIndex.join("/")}`;
}

describe("When static routes are listed", () => {
  it("Then every route has a screen file", () => {
    const screenPaths = listRouteFiles(routerDirectory).map(toUrlPath);
    const staticRoutes = Object.values(routes).flatMap((route) =>
      typeof route === "string" ? [route] : [],
    );

    const routesWithoutScreen = staticRoutes.filter((route) => !screenPaths.includes(route));

    expect(staticRoutes.length).toBeGreaterThan(0);
    expect(routesWithoutScreen).toEqual([]);
  });
});
